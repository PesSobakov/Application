using Microsoft.EntityFrameworkCore;
using Application.Server.Models.DTOs;
using Application.Server.Models.CoworkingDatabase;

namespace Application.Server.Services
{
    public class CoworkingDatabaseService : ICoworkingDatabaseService
    {
        private readonly CoworkingContext _coworkingContext;
        private readonly ITimeProvider _timeProvider;
        public CoworkingDatabaseService(CoworkingContext context, ITimeProvider timeProvider)
        {
            _coworkingContext = context;
            _timeProvider = timeProvider;
        }

        public async Task<ServiceResponse<List<Workspace>>> GetWorkspaces()
        {
            List<Workspace> workspaces = await _coworkingContext.Workspaces
                .Include(workspace => workspace.Bookings)
                .AsSplitQuery()
                .ToListAsync();

            return new ServiceResponse<List<Workspace>>()
            {
                Data = workspaces,
                Status = ResponseStatus.Ok
            };
        }

        public async Task<ServiceResponse<User>> GetUser(string login)
        {
            User? user = await _coworkingContext.Users.Where(user => user.Email == login).FirstOrDefaultAsync();
            if (user == null)
            {
                user = new User()
                {
                    Email = login
                };
                _coworkingContext.Add(user);
                await _coworkingContext.SaveChangesAsync();
            }

            return new ServiceResponse<User>()
            {
                Data = user,
                Status = ResponseStatus.Ok
            };
        }
        public async Task<ServiceResponse<User>> Login(LoginDto loginDto)
        {
            User? user = await _coworkingContext.Users.Where(user => user.Email == loginDto.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                user = new User()
                {
                    Email = loginDto.Email!
                };
                _coworkingContext.Add(user);
                await _coworkingContext.SaveChangesAsync();
            }

            return new ServiceResponse<User>()
            {
                Data = user,
                Status = ResponseStatus.Ok
            };
        }

        public async Task<ServiceResponse> CreateBooking(string login, CreateBookingDto createBookingDto)
        {
            User? user = await _coworkingContext.Users
                    .Where(user => user.Email == login)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            DateTime currentDatetime = _timeProvider.Now();
            DateOnly currentDate = DateOnly.FromDateTime(currentDatetime);
            TimeOnly currentTime = TimeOnly.FromDateTime(currentDatetime);
            if (createBookingDto.StartDate < currentDate)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["Start date must be in future"] };
            }
            if (createBookingDto.EndDate < createBookingDto.StartDate)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["End date must be after or same as start date"] };
            }
            if (createBookingDto.EndTime <= createBookingDto.StartTime)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["End time must be after start time"] };
            }
            if (createBookingDto.StartDate == currentDate && createBookingDto.StartTime < currentTime)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["Start time must be in future"] };
            }
            if (createBookingDto.WorkspaceType == WorkspaceType.MeetingRoom && createBookingDto.StartDate != createBookingDto.EndDate)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["Meeting room can be booked only for 1 day"] };
            }
            else if (createBookingDto.EndDate.CompareTo(createBookingDto.StartDate) > 30-1)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["Maximum booking time is 30 days"] };
            }

            if (createBookingDto.WorkspaceType == WorkspaceType.OpenSpace)
            {
                if (!_coworkingContext.Workspaces.Where(workspace => (workspace.WorkspaceType == createBookingDto.WorkspaceType) && (workspace.Capacity >= createBookingDto.Seats)).Any())
                {
                    return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["No open space big enough "] };
                }
            }
            else
            {
                if (!_coworkingContext.Workspaces.Where(workspace => (workspace.WorkspaceType == createBookingDto.WorkspaceType) && (workspace.Capacity == createBookingDto.Seats)).Any())
                {
                    return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["No room of specified size"] };
                }
            }

            List<Workspace> workspaceCandidates;
            Workspace? workspace;
            if (createBookingDto.WorkspaceType == WorkspaceType.OpenSpace)
            {
                workspaceCandidates = await _coworkingContext.Workspaces
                    .Include(workspace=> workspace.Bookings)
                    .Where(workspace => workspace.WorkspaceType == createBookingDto.WorkspaceType)
                    .ToListAsync();
                workspace = workspaceCandidates
                    .Where(workspace => workspace.Capacity - workspace.Bookings.Where(booking => !((booking.EndDate < createBookingDto.StartDate) || (booking.StartDate > createBookingDto.EndDate))).Count() >= createBookingDto.Seats)
                    .OrderByDescending(workspace => workspace.Bookings.Where(booking => booking.StartDate > createBookingDto.StartDate).Count())
                    .FirstOrDefault();
            }
            else
            {
                workspaceCandidates = await _coworkingContext.Workspaces
                    .Include(workspace => workspace.Bookings)
                    .Where(workspace => workspace.WorkspaceType == createBookingDto.WorkspaceType)
                    .Where(workspace => workspace.Capacity == createBookingDto.Seats)
                    .ToListAsync();
                workspace = workspaceCandidates
                    .Where(workspace => !workspace.Bookings.Where(booking => !((booking.EndDate < createBookingDto.StartDate) || (booking.StartDate > createBookingDto.EndDate))).Any())
                    .OrderByDescending(workspace => workspace.Bookings.Where(booking => booking.StartDate > createBookingDto.StartDate).Count())
                    .FirstOrDefault();
            }

            if (workspace == null)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["Selected time is not available. Please choose a different slot."] };
            }

            Booking booking = new Booking()
            {
                Name = createBookingDto.Name,
                Email = createBookingDto.Email,
                Workspace = workspace,
                User = user,
                Seats = createBookingDto.Seats,
                StartDate = createBookingDto.StartDate,
                EndDate = createBookingDto.EndDate,
                StartTime = createBookingDto.StartTime,
                EndTime = createBookingDto.EndTime
            };

            await _coworkingContext.AddAsync(booking);
            await _coworkingContext.SaveChangesAsync();

            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse> DeleteBooking(string login, int id)
        {
            User? user = await _coworkingContext.Users
                    .Where(user => user.Email == login)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Booking? booking = await _coworkingContext.Bookings
                   .Where(booking => booking.Id == id)
                   .Include(booking => booking.User)
                   .AsSplitQuery()
                   .FirstOrDefaultAsync();
            if (booking == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }
            else if (booking.User.Id != user.Id) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }

            _coworkingContext.Remove(booking);
            await _coworkingContext.SaveChangesAsync();

            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse> EditBooking(string login, int id, EditBookingDto editBookingDto)
        {
            User? user = await _coworkingContext.Users
                    .Where(user => user.Email == login)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Booking? oldBooking = await _coworkingContext.Bookings.Where(booking => booking.Id == id).FirstOrDefaultAsync();
            if (oldBooking == null)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["Booking not exists"] };
            }

            DateTime currentDatetime = _timeProvider.Now();
            DateOnly currentDate = DateOnly.FromDateTime(currentDatetime);
            TimeOnly currentTime = TimeOnly.FromDateTime(currentDatetime);
            if (editBookingDto.StartDate < currentDate)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["Start date must be in future"] };
            }
            if (editBookingDto.EndDate < editBookingDto.StartDate)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["End date must be after or same as start date"] };
            }
            if (editBookingDto.EndTime <= editBookingDto.StartTime)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["End time must be after start time"] };
            }
            if (editBookingDto.StartDate == currentDate && editBookingDto.StartTime < currentTime)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["Start time must be in future"] };
            }
            if (editBookingDto.WorkspaceType == WorkspaceType.MeetingRoom && editBookingDto.StartDate != editBookingDto.EndDate)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["Meeting room can be booked only for 1 day"] };
            }
            else if (editBookingDto.EndDate.CompareTo(editBookingDto.StartDate) > 30)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["Maximum booking time is 30 days"] };
            }

            if (editBookingDto.WorkspaceType == WorkspaceType.OpenSpace)
            {
                if (!_coworkingContext.Workspaces.Where(workspace => (workspace.WorkspaceType == editBookingDto.WorkspaceType) && (workspace.Capacity >= editBookingDto.Seats)).Any())
                {
                    return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["No open space big enough "] };
                }
            }
            else
            {
                if (!_coworkingContext.Workspaces.Where(workspace => (workspace.WorkspaceType == editBookingDto.WorkspaceType) && (workspace.Capacity == editBookingDto.Seats)).Any())
                {
                    return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["No room of specified size"] };
                }
            }

            List<Workspace> workspaceCandidates;
            Workspace? workspace;
            if (editBookingDto.WorkspaceType == WorkspaceType.OpenSpace)
            {
                workspaceCandidates = await _coworkingContext.Workspaces
                    .Include(workspace => workspace.Bookings)
                    .Where(workspace => workspace.WorkspaceType == editBookingDto.WorkspaceType)
                    .ToListAsync();
                workspace = workspaceCandidates
                    .Where(workspace => workspace.Capacity - workspace.Bookings
                        .Where(booking => !((booking.EndDate < editBookingDto.StartDate) || (booking.StartDate > editBookingDto.EndDate)))
                        .Where(booking => booking.Id != oldBooking.Id)
                        .Count() >= editBookingDto.Seats)
                    .OrderByDescending(workspace => workspace.Bookings.Where(booking => booking.StartDate > editBookingDto.StartDate).Count())
                    .FirstOrDefault();
            }
            else
            {
                workspaceCandidates = await _coworkingContext.Workspaces
                    .Include(workspace => workspace.Bookings)
                    .Where(workspace => workspace.WorkspaceType == editBookingDto.WorkspaceType)
                    .Where(workspace => workspace.Capacity == editBookingDto.Seats)
                    .ToListAsync();
                workspace = workspaceCandidates
                    .Where(workspace => !workspace.Bookings
                        .Where(booking => !((booking.EndDate < editBookingDto.StartDate) || (booking.StartDate > editBookingDto.EndDate)))
                        .Where(booking => booking.Id != oldBooking.Id)
                        .Any())
                    .OrderByDescending(workspace => workspace.Bookings.Where(booking => booking.StartDate > editBookingDto.StartDate).Count())
                    .FirstOrDefault();
            }

            if (workspace == null)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["Selected time is not available. Please choose a different slot."] };
            }

            Booking booking = new Booking()
            {
                Name = editBookingDto.Name,
                Email = editBookingDto.Email,
                Workspace = workspace,
                User = user,
                Seats = editBookingDto.Seats,
                StartDate = editBookingDto.StartDate,
                EndDate = editBookingDto.EndDate,
                StartTime = editBookingDto.StartTime,
                EndTime = editBookingDto.EndTime
            };

            _coworkingContext.Remove(oldBooking);
            await _coworkingContext.AddAsync(booking);
            await _coworkingContext.SaveChangesAsync();

            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse<Booking>> GetBooking(string login, int id)
        {
            User? user = await _coworkingContext.Users
                    .Where(user => user.Email == login)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Booking? booking = await _coworkingContext.Bookings
                   .Where(booking => booking.Id == id)
                   .Include(booking => booking.Workspace)
                   .AsSplitQuery()
                   .FirstOrDefaultAsync();
            if (booking == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }
            else if (booking.User.Id != user.Id) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }

            return new ServiceResponse<Booking>()
            {
                Data = booking,
                Status = ResponseStatus.Ok
            };
        }
        public async Task<ServiceResponse<List<Booking>>> GetBookings(string login)
        {
            User? user = await _coworkingContext.Users
                .Where(user => user.Email == login)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            List<Booking> bookings = await _coworkingContext.Bookings
                .Where(booking => booking.User.Id == user.Id)
                .Include(booking => booking.Workspace)
                .AsSplitQuery()
                .ToListAsync();

            return new ServiceResponse<List<Booking>>()
            {
                Data = bookings,
                Status = ResponseStatus.Ok
            };
        }

        public async Task<ServiceResponse> Seed()
        {
            await _coworkingContext.Database.EnsureDeletedAsync();
            await _coworkingContext.Database.EnsureCreatedAsync();

            List<Workspace> workspaces =
            [
                new Workspace
                {
                    Capacity = 24,
                    Amenities = [Amenity.Coffee, Amenity.GameRoom, Amenity.WiFi, Amenity.Conditioner],
                    WorkspaceType = WorkspaceType.OpenSpace
                },
                new Workspace
                {
                    Capacity = 1,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 1,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 1,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 1,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 1,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 1,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 1,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 2,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 2,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 2,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 2,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 5,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 5,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 5,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 10,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.PrivateRoom
                },
                new Workspace
                {
                    Capacity = 10,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Microphone, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.MeetingRoom
                },
                new Workspace
                {
                    Capacity = 10,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Microphone, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.MeetingRoom
                },
                new Workspace
                {
                    Capacity = 10,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Microphone, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.MeetingRoom
                },
                new Workspace
                {
                    Capacity = 10,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Microphone, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.MeetingRoom
                },
                new Workspace
                {
                    Capacity = 20,
                    Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Microphone, Amenity.Headphones],
                    WorkspaceType = WorkspaceType.MeetingRoom
                }
            ];
            List<User> users =
            [
                new User
                {
                    Email = "user@example.com",
                    Bookings =
                    [
                        new Booking(){
                            Name = "John Snow",
                            Email = "user@example.com",
                            Workspace = workspaces[0],
                            Seats = 1,
                            StartDate = new DateOnly(2025,5,30),
                            EndDate = new DateOnly(2025,6,26),
                            StartTime = new TimeOnly(12,0,0),
                            EndTime = new TimeOnly(16,0,0)
                        },
                        new Booking(){
                            Name = "John Snow",
                            Email = "user@example.com",
                            Workspace = workspaces[1],
                            Seats = 1,
                            StartDate = new DateOnly(2025,6,1),
                            EndDate = new DateOnly(2025,6,20),
                            StartTime = new TimeOnly(8,0,0),
                            EndTime = new TimeOnly(12,0,0)
                        }
                    ]
                },
                new User{
                    Email = "user2@example.com",
                    Bookings =
                    [
                        new Booking(){
                            Name = "Bill Water",
                            Email = "user2@example.com",
                            Workspace = workspaces[20],
                            Seats = 10,
                            StartDate = new DateOnly(2025,6,3),
                            EndDate = new DateOnly(2025,6,3),
                            StartTime = new TimeOnly(8,0,0),
                            EndTime = new TimeOnly(14,0,0)
                        }
                    ]
                }
            ];

            _coworkingContext.AddRange(users);
            _coworkingContext.AddRange(workspaces);
            await _coworkingContext.SaveChangesAsync();

            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
    }
}
