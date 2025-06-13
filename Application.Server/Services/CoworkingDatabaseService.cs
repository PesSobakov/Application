using Microsoft.EntityFrameworkCore;
using Application.Server.Models.DTOs;
using Application.Server.Models.CoworkingDatabase;
using Application.Server.Models.DTOs.GetCoworkings;
using System.Collections.Generic;
using Application.Server.Models.DTOs.GetWorkspaces;
using System.Text.Json;
using System.Net;
using AutoMapper;
using Application.Server.Models.DTOs.GetBooking;
using System.Linq;
using System.Globalization;

namespace Application.Server.Services
{
    public class CoworkingDatabaseService : ICoworkingDatabaseService
    {
        private readonly CoworkingContext _coworkingContext;
        private readonly ITimeProvider _timeProvider;
        private readonly IGroqService _groqService;
        private readonly IMapper _mapper;
        public CoworkingDatabaseService(CoworkingContext context, ITimeProvider timeProvider, IGroqService groqService, IMapper mapper)
        {
            _coworkingContext = context;
            _timeProvider = timeProvider;
            _groqService = groqService;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<Models.DTOs.GetCoworkings.CoworkingDto>>> GetCoworkings()
        {
            List<Coworking> coworkings = await _coworkingContext.Coworkings
                .Include(coworking => coworking.Workspaces).ThenInclude(workspace => workspace.Bookings)
                .AsSplitQuery()
                .ToListAsync();

            DateTime currentDatetime = _timeProvider.Now();
            DateOnly currentDate = DateOnly.FromDateTime(currentDatetime);

            List<Models.DTOs.GetCoworkings.CoworkingDto> coworkingDtos = coworkings.Select(coworking => new Models.DTOs.GetCoworkings.CoworkingDto()
            {
                Id = coworking.Id,
                Name = coworking.Name,
                Description = coworking.Description,
                Address = coworking.Address,
                RoomCountDtos = coworking.Workspaces.GroupBy(workspace => workspace.WorkspaceType).Select(group =>
                {
                    if (group.Key == WorkspaceType.OpenSpace)
                    {
                        return new RoomCountDto()
                        {
                            WorkspaceType = group.Key,
                            Rooms = group.Select(workspace => workspace.Capacity - workspace.Bookings
                                .Where(booking => !((booking.EndDate < currentDate) || (booking.StartDate > currentDate)))
                                .Sum(booking => booking.Seats)).Sum()
                        };
                    }
                    else
                    {
                        return new RoomCountDto()
                        {
                            WorkspaceType = group.Key,
                            Rooms = group.Where(workspace => !workspace.Bookings.Where(booking => !((booking.EndDate < currentDate) || (booking.StartDate > currentDate))).Any()).Count()
                        };
                    }
                }).ToList()
            }).ToList();

            return new ServiceResponse<List<Models.DTOs.GetCoworkings.CoworkingDto>>()
            {
                Data = coworkingDtos,
                Status = ResponseStatus.Ok
            };

        }
        public async Task<ServiceResponse<List<WorkspaceGroupDto>>> GetWorkspaces(string? login, int id)
        {
            User? user = null;
            if (login != null)
            {
                user = await _coworkingContext.Users
                .Where(user => user.Email == login)
                .FirstOrDefaultAsync();
            }

            Coworking? coworking = await _coworkingContext.Coworkings
                .Where(coworking => coworking.Id == id)
                .Include(coworking => coworking.Workspaces).ThenInclude(workspace => workspace.Bookings).ThenInclude(booking => booking.User)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (coworking == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }


            List<WorkspaceGroupDto> workspaceGroupDtos;
            DateTime currentDatetime = _timeProvider.Now();
            DateOnly currentDate = DateOnly.FromDateTime(currentDatetime);

            workspaceGroupDtos = coworking.Workspaces.GroupBy(workspace => workspace.WorkspaceType).Select(group =>
            {
                if (group.Key == WorkspaceType.OpenSpace)
                {
                    return new WorkspaceGroupDto()
                    {
                        WorkspaceType = group.Key,
                        Amenities = group.First().Amenities,
                        FreeRooms = new List<FreeRoomsDto>(){ new FreeRoomsDto() {
                                Capacity = 1,
                                Rooms = group.Select(workspace => workspace.Capacity - workspace.Bookings
                                .Where(booking => !((booking.EndDate < currentDate) || (booking.StartDate > currentDate)))
                                .Sum(booking => booking.Seats)).Sum()
                            } },
                        Bookings = group.SelectMany(workspace => workspace.Bookings).Where(booking => booking.User.Id == user?.Id).Where(booking => booking.EndDate >= currentDate).Select(booking => new Models.DTOs.GetWorkspaces.BookingDto()
                        {
                            Id = booking.Id,
                            Seats = booking.Seats,
                            StartDate = booking.StartDate,
                            EndDate = booking.EndDate,
                            StartTime = booking.StartTime,
                            EndTime = booking.EndTime
                        }).ToList()
                    };
                }
                else
                {
                    return new WorkspaceGroupDto()
                    {
                        WorkspaceType = group.Key,
                        Amenities = group.First().Amenities,
                        FreeRooms = group.GroupBy(workspace => workspace.Capacity).Select(group => new FreeRoomsDto()
                        {
                            Capacity = group.Key,
                            Rooms = group.Where(workspace => !workspace.Bookings.Where(booking => !((booking.EndDate < currentDate) || (booking.StartDate > currentDate))).Any()).Count()
                        }).ToList(),
                        Bookings = group.SelectMany(workspace => workspace.Bookings).Where(booking => booking.User.Id == user?.Id).Where(booking => booking.EndDate >= currentDate).Select(booking => new Models.DTOs.GetWorkspaces.BookingDto()
                        {
                            Id = booking.Id,
                            Seats = booking.Seats,
                            StartDate = booking.StartDate,
                            EndDate = booking.EndDate,
                            StartTime = booking.StartTime,
                            EndTime = booking.EndTime
                        }).ToList()
                    };
                }
            }).ToList();

            return new ServiceResponse<List<WorkspaceGroupDto>>()
            {
                Data = workspaceGroupDtos,
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
                    .FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }
            Coworking? coworking = await _coworkingContext.Coworkings
                    .Where(coworking => coworking.Id == createBookingDto.CoworkingId)
                    .FirstOrDefaultAsync();
            if (coworking == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["Coworking not exists"] }; }

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
            else if (createBookingDto.EndDate.CompareTo(createBookingDto.StartDate) > 30 - 1)
            {
                return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["Maximum booking time is 30 days"] };
            }

            if (createBookingDto.WorkspaceType == WorkspaceType.OpenSpace)
            {
                if (!_coworkingContext.Coworkings
                    .Where(coworking => coworking.Id == createBookingDto.CoworkingId)
                    .SelectMany(coworking => coworking.Workspaces)
                    .Where(workspace => (workspace.WorkspaceType == createBookingDto.WorkspaceType) && (workspace.Capacity >= createBookingDto.Seats)).Any())
                {
                    return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["No open space big enough "] };
                }
            }
            else
            {
                if (!_coworkingContext.Coworkings
                    .Where(coworking => coworking.Id == createBookingDto.CoworkingId)
                    .SelectMany(coworking => coworking.Workspaces)
                    .Where(workspace => (workspace.WorkspaceType == createBookingDto.WorkspaceType) && (workspace.Capacity == createBookingDto.Seats)).Any())
                {
                    return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["No room of specified size"] };
                }
            }

            List<Workspace> workspaceCandidates;
            Workspace? workspace;
            if (createBookingDto.WorkspaceType == WorkspaceType.OpenSpace)
            {
                workspaceCandidates = await _coworkingContext.Coworkings
                    .Where(coworking => coworking.Id == createBookingDto.CoworkingId)
                    .SelectMany(coworking => coworking.Workspaces)
                    .Include(workspace => workspace.Bookings)
                    .Where(workspace => workspace.WorkspaceType == createBookingDto.WorkspaceType)
                    .ToListAsync();
                workspace = workspaceCandidates
                    .Where(workspace => workspace.Capacity - workspace.Bookings
                        .Where(booking => !((booking.EndDate < createBookingDto.StartDate) || (booking.StartDate > createBookingDto.EndDate)))
                        .Sum(booking => booking.Seats) >= createBookingDto.Seats)
                    .OrderByDescending(workspace => workspace.Bookings.Where(booking => booking.StartDate > createBookingDto.StartDate).Count())
                    .FirstOrDefault();
            }
            else
            {
                workspaceCandidates = await _coworkingContext.Coworkings
                    .Where(coworking => coworking.Id == createBookingDto.CoworkingId)
                    .SelectMany(coworking => coworking.Workspaces)
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

            Booking? oldBooking = await _coworkingContext.Bookings
                .Where(booking => booking.Id == id)
                .Include(booking=>booking .Workspace).ThenInclude(workspace=> workspace.Coworking)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
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
                if (!_coworkingContext.Coworkings
                    .Where(coworking => coworking.Id == oldBooking.Workspace.Coworking.Id)
                    .SelectMany(coworking => coworking.Workspaces)
                    .Where(workspace => (workspace.WorkspaceType == editBookingDto.WorkspaceType) && (workspace.Capacity >= editBookingDto.Seats)).Any())
                {
                    return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["No open space big enough "] };
                }
            }
            else
            {
                if (!_coworkingContext.Coworkings
                    .Where(coworking => coworking.Id == oldBooking.Workspace.Coworking.Id)
                    .SelectMany(coworking => coworking.Workspaces)
                    .Where(workspace => (workspace.WorkspaceType == editBookingDto.WorkspaceType) && (workspace.Capacity == editBookingDto.Seats)).Any())
                {
                    return new ServiceResponse() { Status = ResponseStatus.BadRequest, ErrorMessages = ["No room of specified size"] };
                }
            }

            List<Workspace> workspaceCandidates;
            Workspace? workspace;
            if (editBookingDto.WorkspaceType == WorkspaceType.OpenSpace)
            {
                workspaceCandidates = await _coworkingContext.Coworkings
                    .Where(coworking => coworking.Id == oldBooking.Workspace.Coworking.Id)
                    .SelectMany(coworking => coworking.Workspaces)
                    .Include(workspace => workspace.Bookings)
                    .Where(workspace => workspace.WorkspaceType == editBookingDto.WorkspaceType)
                    .ToListAsync();
                workspace = workspaceCandidates
                    .Where(workspace => workspace.Capacity - workspace.Bookings
                        .Where(booking => !((booking.EndDate < editBookingDto.StartDate) || (booking.StartDate > editBookingDto.EndDate)))
                        .Where(booking => booking.Id != oldBooking.Id)
                        .Sum(booking => booking.Seats) >= editBookingDto.Seats)
                    .OrderByDescending(workspace => workspace.Bookings.Where(booking => booking.StartDate > editBookingDto.StartDate).Count())
                    .FirstOrDefault();
            }
            else
            {
                workspaceCandidates = await _coworkingContext.Coworkings
                    .Where(coworking => coworking.Id == oldBooking.Workspace.Coworking.Id)
                    .SelectMany(coworking => coworking.Workspaces)
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
                   .Include(booking => booking.Workspace).ThenInclude(workspace => workspace.Coworking)
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
                .Include(booking => booking.Workspace).ThenInclude(workspace => workspace.Coworking)
                .AsSplitQuery()
                .ToListAsync();

            return new ServiceResponse<List<Booking>>()
            {
                Data = bookings,
                Status = ResponseStatus.Ok
            };
        }

        public async Task<ServiceResponse<StringDto>> BookingsQuestion(string login, StringDto question)
        {
            User? user = await _coworkingContext.Users
                .Where(user => user.Email == login)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            List<Booking> bookings = await _coworkingContext.Bookings
                .Where(booking => booking.User.Id == user.Id)
                .Include(booking => booking.Workspace).ThenInclude(workspace => workspace.Coworking)
                .AsSplitQuery()
                .ToListAsync();

            DateTime currentDateTime = _timeProvider.Now();
            DateOnly currentDate = DateOnly.FromDateTime(currentDateTime);

            string systemPrompt = "You are assistant that can search or filter bookings following to request.\r\nYou cannot respond to any other questions.\r\nrespond with JSON with following structure\r\n{\"type\": \"list|count|unknown\" , \"count\": \"number\", \"data\" : [\"number\",\"number\",\"number\", ...]}\r\nFor example {\"type\": \"list\" , \"data\" : [1,2,3]} {\"type\": \"count\" , \"count\": 5, \"data\" : [4,5,6,7,8]} {\"type\": \"unknown\"}\r\nWhen you need to list bookings, write their IDs {\"type\": \"list\" , \"data\" : [1,2,3]}.\r\nWhen you need to count bookings, write their IDs write one number {\"type\": \"count\" , \"count\": 5, \"data\" : [4,5,6,7,8]}.\r\nIn case of not clear or forbidden question you must respond with message {\"type\": \"unknown\"}.\r\nWorkspaceType is enum {\"Open Space\": 0,\"Private room\": 1,\"Meeting room\": 2}.\r\nWhen you need to tell when something was booked use start date as booking date.\r\nif nothing found respond with {\"type\": \"list\" , \"data\" : []} or {\"type\": \"count\" , \"count\": 0, \"data\" : []} depending on request type.\r\nif you need to say if something exists response type is list.";

            List<Models.DTOs.BookingQuestion.BookingDto> dto = bookings.Select(_mapper.Map<Models.DTOs.BookingQuestion.BookingDto>).ToList();
            string JsonData = JsonSerializer.Serialize(dto, new JsonSerializerOptions() { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles, WriteIndented = true });
            string inputData = "Current Date " + currentDate + "\r\nDateBookings data: " + JsonData;

            var response = await _groqService.MakeRequest(systemPrompt, inputData, question.Value);

            if (response.Status != ResponseStatus.Ok) { return new ServiceResponse() { Status = response .Status} ; }

            BookingQuestionResponseDto? aiResponse = JsonSerializer.Deserialize<BookingQuestionResponseDto>(response.Data!);
            if (aiResponse == null) { return new ServiceResponse() { Status = ResponseStatus.InternalServerError }; }

            string formattedResponse;
            switch (aiResponse.Type)
            {
                case "list":
                    if (aiResponse.Data == null)
                    {
                        formattedResponse = "Sorry, I didn’t understand that. Please try rephrasing your question";
                    }
                    else if (aiResponse.Data.Count == 0)
                    {
                        formattedResponse = "You have no bookings meeting the criteria";
                    }
                    else
                    {
                        formattedResponse = string.Join('\n', bookings
                            .Where(booking => aiResponse.Data.IndexOf(booking.Id) >= 0)
                            .Select(booking =>
                            {
                                string date = booking.StartDate == booking.EndDate ? booking.StartDate.ToString("yyyy-MM-dd") : $"From {booking.StartDate.ToString("yyyy-MM-dd")} to {booking.EndDate.ToString("yyyy-MM-dd")}";
                                string workspaceType = booking.Workspace.WorkspaceType switch
                                {
                                    WorkspaceType.OpenSpace => "Open space",
                                    WorkspaceType.PrivateRoom => "Private room",
                                    WorkspaceType.MeetingRoom => "Meeting room",
                                    _ => ""
                                };
                                string seats = $"{booking.Seats} {(booking.Seats == 1 ? "person" : "people")}";
                                return $"📅 {date} — {workspaceType} for {seats} at {booking.Workspace.Coworking.Name} ({booking.StartTime.ToString("HH':'mm")} – {booking.EndTime.ToString("HH':'mm")})";
                            })
                        );
                    }
                    break;
                case "count":
                    if (aiResponse.Data == null || aiResponse.Count == null)
                    {
                        formattedResponse = "Sorry, I didn’t understand that. Please try rephrasing your question";
                    }
                    else if (aiResponse.Data.Count == aiResponse.Count)
                    {
                        formattedResponse = aiResponse.Count.Value.ToString();
                    }
                    else
                    {
                        formattedResponse = "Sorry, I didn’t understand that. Please try rephrasing your question";
                    }
                    break;
                case "unknown":
                    formattedResponse = "Sorry, I didn’t understand that. Please try rephrasing your question";
                    break;
                default:
                    formattedResponse = "Sorry, I didn’t understand that. Please try rephrasing your question";
                    break;
            }
            return new ServiceResponse<StringDto>()
            {
                Status = ResponseStatus.Ok,
                Data = new StringDto() { Value= formattedResponse }
            };
        }

        public async Task<ServiceResponse> Seed()
        {
            await _coworkingContext.Database.EnsureCreatedAsync();
            _coworkingContext.RemoveRange(_coworkingContext.Coworkings);
            _coworkingContext.RemoveRange(_coworkingContext.Workspaces);
            _coworkingContext.RemoveRange(_coworkingContext.Bookings);
            _coworkingContext.RemoveRange(_coworkingContext.Users);

            List<Coworking> coworkings =
            [
                new Coworking(){
                    Name="WorkClub Pechersk",
                    Description ="Modern coworking in the heart of Pechersk with quiet rooms and coffee on tap.",
                    Address="123 Yaroslaviv Val St, Kyiv",
                    Workspaces=
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
                    ]
                },
                new Coworking(){
                    Name="UrbanSpace Podil",
                    Description ="A creative riverside hub ideal for freelancers and small startups.",
                    Address="78 Naberezhno-Khreshchatytska St, Kyiv",
                    Workspaces=
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
                    ]
                },
                new Coworking(){
                    Name="Creative Hub Lvivska",
                    Description ="A compact, design-focused space with open desks and strong community vibes.",
                    Address="12 Lvivska Square, Kyiv",
                    Workspaces=
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
                    ]
                },
                new Coworking(){
                    Name="TechNest Olimpiiska",
                    Description ="A high-tech space near Olimpiiska metro, perfect for team sprints and solo focus.",
                    Address="45 Velyka Vasylkivska St, Kyiv",
                    Workspaces=
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
                    ]
                },
                new Coworking(){
                    Name="Hive Station Troieshchyna",
                    Description ="A quiet, affordable option in the city's northeast—great for remote workers.",
                    Address="102 Zakrevskogo St, Kyiv",
                    Workspaces=
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
                    ]
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
                            Workspace = coworkings[0].Workspaces[0],
                            Seats = 1,
                            StartDate = new DateOnly(2025,5,30),
                            EndDate = new DateOnly(2025,6,26),
                            StartTime = new TimeOnly(12,0,0),
                            EndTime = new TimeOnly(16,0,0)
                        },
                        new Booking(){
                            Name = "John Snow",
                            Email = "user@example.com",
                            Workspace = coworkings[0].Workspaces[1],
                            Seats = 1,
                            StartDate = new DateOnly(2025,6,1),
                            EndDate = new DateOnly(2025,6,20),
                            StartTime = new TimeOnly(8,0,0),
                            EndTime = new TimeOnly(12,0,0)
                        },
                        new Booking(){
                            Name = "John Snow",
                            Email = "user@example.com",
                            Workspace = coworkings[1].Workspaces[1],
                            Seats = 1,
                            StartDate = new DateOnly(2025,5,1),
                            EndDate = new DateOnly(2025,5,10),
                            StartTime = new TimeOnly(8,0,0),
                            EndTime = new TimeOnly(12,0,0)
                        },
                        new Booking(){
                            Name = "John Snow",
                            Email = "user@example.com",
                            Workspace = coworkings[2].Workspaces[9],
                            Seats = 20,
                            StartDate = new DateOnly(2025,7,1),
                            EndDate = new DateOnly(2025,7,1),
                            StartTime = new TimeOnly(8,0,0),
                            EndTime = new TimeOnly(15,0,0)
                        },
                        new Booking(){
                            Name = "John Snow",
                            Email = "user@example.com",
                            Workspace = coworkings[3].Workspaces[8],
                            Seats = 10,
                            StartDate = new DateOnly(2025,7,2),
                            EndDate = new DateOnly(2025,7,2),
                            StartTime = new TimeOnly(10,0,0),
                            EndTime = new TimeOnly(15,0,0)
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
                            Workspace = coworkings[0].Workspaces[20],
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
            _coworkingContext.AddRange(coworkings);
            await _coworkingContext.SaveChangesAsync();

            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
    }
}
