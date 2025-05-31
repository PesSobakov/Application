using Application.Server.Models.CoworkingDatabase;
using Application.Server.Models.DTOs;
using Application.Server.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace TaskBoard.ServerTest
{
    public class CoworkingDatabaseServiceTest : IDisposable
    {
        SqliteConnection _connection;
        DbContextOptions<CoworkingContext> _contextOptions;
        ICoworkingDatabaseService _coworkingDatabaseService;
        TestTimeProvider _testTimeProvider;
        public CoworkingDatabaseServiceTest()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();
            _contextOptions = new DbContextOptionsBuilder<CoworkingContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new CoworkingContext(_contextOptions);
            context.Database.EnsureCreated();

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

            context.AddRange(users);
            context.AddRange(workspaces);
            context.SaveChanges();

            _testTimeProvider = new TestTimeProvider();
            _testTimeProvider.SetTime(new DateTime(2025, 05, 29, 0, 0, 0));

            _coworkingDatabaseService = new CoworkingDatabaseService(CreateContext(), _testTimeProvider);
        }
        CoworkingContext CreateContext() => new CoworkingContext(_contextOptions);
        public void Dispose() => _connection.Dispose();

        [Fact]
        public async Task GetWorkspaces()
        {
            var context = CreateContext();
            List<Workspace> expected = context.Workspaces
                .Include(workspace => workspace.Bookings)
                .AsSplitQuery()
                .ToList();

            var response = await _coworkingDatabaseService.GetWorkspaces();

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.NotNull(response.Data);
            expected.ForEach(workspace => workspace.Bookings.ForEach(booking => booking.Workspace = null!));
            response.Data.ForEach(workspace => workspace.Bookings.ForEach(booking => booking.Workspace = null!));
            Assert.Equivalent(expected, response.Data);
        }

        [Fact]
        public async Task GetUser()
        {
            User expected = new User
            {
                Id = 1,
                Email = "user@example.com"
            };

            var response = await _coworkingDatabaseService.GetUser("user@example.com");

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.NotNull(response.Data);
            Assert.Equivalent(expected,response.Data);
        }

        [Fact]
        public async Task Login()
        {
            User expected = new User
            {
                Id = 3,
                Email = "1user@example.com"
            };
            LoginDto loginDto = new()
            {
                Email = "1user@example.com"
            };

            var response = await _coworkingDatabaseService.Login(loginDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.NotNull(response.Data);
            Assert.Equivalent(expected, response.Data);
        }

        [Fact]
        public async Task CreateBooking()
        {
            var context = CreateContext();
            CreateBookingDto dto = new CreateBookingDto()
            {
                Name = "John Snow",
                Email = "user@example.com",
                WorkspaceType = WorkspaceType.MeetingRoom,
                Seats = 20,
                StartDate = new DateOnly(2025, 05, 29),
                EndDate = new DateOnly(2025, 05, 29),
                StartTime = new TimeOnly(8, 0, 0),
                EndTime = new TimeOnly(10, 0, 0),
            };

            var response = await _coworkingDatabaseService.CreateBooking("user@example.com", dto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Bookings
                    .Where(booking => booking.Name == "John Snow")
                    .Where(booking => booking.Email == "user@example.com")
                    .Where(booking => booking.Workspace.WorkspaceType == WorkspaceType.MeetingRoom)
                    .Where(booking => booking.Seats == 20)
                    .Where(booking => booking.StartDate == new DateOnly(2025, 05, 29))
                    .Where(booking => booking.EndDate == new DateOnly(2025, 05, 29))
                    .Where(booking => booking.StartTime == new TimeOnly(8, 0, 0))
                    .Where(booking => booking.EndTime == new TimeOnly(10, 0, 0))
                    .Any()
            );
        }

        [Fact]
        public async Task DeleteBooking()
        {
            var context = CreateContext();

            var response = await _coworkingDatabaseService.DeleteBooking("user@example.com", 1);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.False(
                context.Bookings
                    .Where(booking => booking.Id == 1)
                    .Any()
            );

        }
        [Fact]
        public async Task EditBooking()
        {
            var context = CreateContext();
            EditBookingDto dto = new EditBookingDto()
            {
                Name = "John Snow",
                Email = "user@example.com",
                WorkspaceType = WorkspaceType.MeetingRoom,
                Seats = 20,
                StartDate = new DateOnly(2025, 05, 29),
                EndDate = new DateOnly(2025, 05, 29),
                StartTime = new TimeOnly(8, 0, 0),
                EndTime = new TimeOnly(10, 0, 0),
            };

            var response = await _coworkingDatabaseService.EditBooking("user@example.com", 2, dto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.False(
                context.Bookings
                    .Where(booking => booking.Id == 2)
                    .Any()
            );
            Assert.True(
                context.Bookings
                    .Where(booking => booking.Name == "John Snow")
                    .Where(booking => booking.Email == "user@example.com")
                    .Where(booking => booking.Workspace.WorkspaceType == WorkspaceType.MeetingRoom)
                    .Where(booking => booking.Seats == 20)
                    .Where(booking => booking.StartDate == new DateOnly(2025, 05, 29))
                    .Where(booking => booking.EndDate == new DateOnly(2025, 05, 29))
                    .Where(booking => booking.StartTime == new TimeOnly(8, 0, 0))
                    .Where(booking => booking.EndTime == new TimeOnly(10, 0, 0))
                    .Any()
            );
        }
        [Fact]
        public async Task GetBooking()
        {
            Booking expected = new Booking()
            {
                Id = 1,
                Name = "John Snow",
                Email = "user@example.com",
                Workspace = new Workspace
                {
                    Id = 1,
                    Capacity = 24,
                    Amenities = [Amenity.Coffee, Amenity.GameRoom, Amenity.WiFi, Amenity.Conditioner],
                    WorkspaceType = WorkspaceType.OpenSpace
                },
                Seats = 1,
                StartDate = new DateOnly(2025, 5, 30),
                EndDate = new DateOnly(2025, 6, 26),
                StartTime = new TimeOnly(12, 0, 0),
                EndTime = new TimeOnly(16, 0, 0)
            };

            var response = await _coworkingDatabaseService.GetBooking("user@example.com", 1);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.NotNull(response.Data);
            response.Data.User = null!;
            response.Data.Workspace.Bookings = null!;
            Assert.Equivalent(expected, response.Data);
        }

        [Fact]
        public async Task GetBookings()
        {
            List<Booking> expected = [
                new Booking{
                    Id = 1,
                    Name = "John Snow",
                    Email = "user@example.com",
                    Workspace = new Workspace{
                        Id = 1,
                        Capacity = 24,
                        Amenities = [Amenity.Coffee, Amenity.GameRoom, Amenity.WiFi, Amenity.Conditioner],
                        WorkspaceType = WorkspaceType.OpenSpace
                    },
                    Seats = 1,
                    StartDate = new DateOnly(2025,5,30),
                    EndDate = new DateOnly(2025,6,26),
                    StartTime = new TimeOnly(12,0,0),
                    EndTime = new TimeOnly(16,0,0)
                },
                new Booking{
                    Id = 2,
                    Name = "John Snow",
                    Email = "user@example.com",
                    Workspace = new Workspace{
                        Id = 2,
                        Capacity = 1,
                        Amenities = [Amenity.WiFi, Amenity.Conditioner, Amenity.Headphones],
                        WorkspaceType = WorkspaceType.PrivateRoom
                    },
                    Seats = 1,
                    StartDate = new DateOnly(2025,6,1),
                    EndDate = new DateOnly(2025,6,20),
                    StartTime = new TimeOnly(8,0,0),
                    EndTime = new TimeOnly(12,0,0)
                }
            ];
        
            var response = await _coworkingDatabaseService.GetBookings("user@example.com");

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.NotNull(response.Data);
            response.Data.ForEach(booking => booking.Workspace.Bookings = null!);
            response.Data.ForEach(booking => booking.User = null!);
            Assert.Equivalent(expected, response.Data);
        }
    }
}
