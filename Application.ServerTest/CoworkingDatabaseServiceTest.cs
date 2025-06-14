using Application.Server.Models.CoworkingDatabase;
using Application.Server.Models.DTOs;
using Application.Server.Models.DTOs.GetCoworkings;
using Application.Server.Models.DTOs.GetWorkspaces;
using Application.Server.Models.Mapper;
using Application.Server.Services;
using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace TaskBoard.ServerTest
{
    public class CoworkingDatabaseServiceTest : IDisposable
    {
        SqliteConnection _connection;
        DbContextOptions<CoworkingContext> _contextOptions;
        ICoworkingDatabaseService _coworkingDatabaseService;
        TestTimeProvider _testTimeProvider;
        TestGroqService _testGroqService;
        IMapper _mapper;
        public CoworkingDatabaseServiceTest()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();
            _contextOptions = new DbContextOptionsBuilder<CoworkingContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new CoworkingContext(_contextOptions);
            context.Database.EnsureCreated();

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

            context.AddRange(users);
            context.AddRange(coworkings);
            context.SaveChanges();

            _testTimeProvider = new TestTimeProvider();
            _testTimeProvider.SetTime(new DateTime(2025, 05, 29, 0, 0, 0));

            _testGroqService = new TestGroqService();

            var myProfile = new CoworkingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            _mapper = new Mapper(configuration);

            _coworkingDatabaseService = new CoworkingDatabaseService(CreateContext(), _testTimeProvider, _testGroqService, _mapper);
        }
        CoworkingContext CreateContext() => new CoworkingContext(_contextOptions);
        public void Dispose() => _connection.Dispose();

        [Fact]
        public async Task GetCoworkings()
        {
            var expected = JsonSerializer.Deserialize<List<CoworkingDto>>("[{\"Id\":1,\"Name\":\"WorkClub Pechersk\",\"Description\":\"Modern coworking in the heart of Pechersk with quiet rooms and coffee on tap.\",\"Address\":\"123 Yaroslaviv Val St, Kyiv\",\"RoomCountDtos\":[{\"WorkspaceType\":0,\"Rooms\":24},{\"WorkspaceType\":1,\"Rooms\":15},{\"WorkspaceType\":2,\"Rooms\":5}]},{\"Id\":2,\"Name\":\"UrbanSpace Podil\",\"Description\":\"A creative riverside hub ideal for freelancers and small startups.\",\"Address\":\"78 Naberezhno-Khreshchatytska St, Kyiv\",\"RoomCountDtos\":[{\"WorkspaceType\":1,\"Rooms\":6},{\"WorkspaceType\":0,\"Rooms\":24},{\"WorkspaceType\":2,\"Rooms\":3}]},{\"Id\":3,\"Name\":\"Creative Hub Lvivska\",\"Description\":\"A compact, design-focused space with open desks and strong community vibes.\",\"Address\":\"12 Lvivska Square, Kyiv\",\"RoomCountDtos\":[{\"WorkspaceType\":2,\"Rooms\":3},{\"WorkspaceType\":0,\"Rooms\":24},{\"WorkspaceType\":1,\"Rooms\":6}]},{\"Id\":4,\"Name\":\"TechNest Olimpiiska\",\"Description\":\"A high-tech space near Olimpiiska metro, perfect for team sprints and solo focus.\",\"Address\":\"45 Velyka Vasylkivska St, Kyiv\",\"RoomCountDtos\":[{\"WorkspaceType\":2,\"Rooms\":3},{\"WorkspaceType\":0,\"Rooms\":24},{\"WorkspaceType\":1,\"Rooms\":6}]},{\"Id\":5,\"Name\":\"Hive Station Troieshchyna\",\"Description\":\"A quiet, affordable option in the city\\u0027s northeast\\u2014great for remote workers.\",\"Address\":\"102 Zakrevskogo St, Kyiv\",\"RoomCountDtos\":[{\"WorkspaceType\":0,\"Rooms\":24},{\"WorkspaceType\":1,\"Rooms\":6},{\"WorkspaceType\":2,\"Rooms\":3}]}]");
           
            var response = await _coworkingDatabaseService.GetCoworkings();
         
            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.NotNull(response.Data);
            Assert.Equivalent(expected, response.Data);
        }

        [Fact]
        public async Task GetWorkspaces()
        {
            var expected = JsonSerializer.Deserialize<List<WorkspaceGroupDto>>("[{\"WorkspaceType\":0,\"Amenities\":[0,1,2,3],\"FreeRooms\":[{\"Capacity\":1,\"Rooms\":24}],\"Bookings\":[{\"Id\":1,\"Seats\":1,\"StartDate\":\"2025-05-30\",\"EndDate\":\"2025-06-26\",\"StartTime\":\"12:00:00\",\"EndTime\":\"16:00:00\"}]},{\"WorkspaceType\":1,\"Amenities\":[2,3,5],\"FreeRooms\":[{\"Capacity\":1,\"Rooms\":7},{\"Capacity\":2,\"Rooms\":4},{\"Capacity\":5,\"Rooms\":3},{\"Capacity\":10,\"Rooms\":1}],\"Bookings\":[{\"Id\":2,\"Seats\":1,\"StartDate\":\"2025-06-01\",\"EndDate\":\"2025-06-20\",\"StartTime\":\"08:00:00\",\"EndTime\":\"12:00:00\"}]},{\"WorkspaceType\":2,\"Amenities\":[2,3,4,5],\"FreeRooms\":[{\"Capacity\":20,\"Rooms\":1},{\"Capacity\":10,\"Rooms\":4}],\"Bookings\":[]}]");

            var response = await _coworkingDatabaseService.GetWorkspaces("user@example.com", 1);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.NotNull(response.Data);
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
            Assert.Equivalent(expected, response.Data);
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
                CoworkingId = 1,
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
                    .Where(booking => booking.Workspace.Coworking.Id == 1)
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
                    WorkspaceType = WorkspaceType.OpenSpace,
                    Coworking = new Coworking()
                    {
                        Id = 1,
                        Name = "WorkClub Pechersk",
                        Description = "Modern coworking in the heart of Pechersk with quiet rooms and coffee on tap.",
                        Address = "123 Yaroslaviv Val St, Kyiv"
                    }
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
            response.Data.Workspace.Coworking.Workspaces = null!;
            Assert.Equivalent(expected, response.Data);
        }

        [Fact]
        public async Task GetBookings()
        {
            var expected = JsonSerializer.Deserialize<List<Booking>>("[{\"Id\":1,\"Name\":\"John Snow\",\"Email\":\"user@example.com\",\"User\":null,\"Workspace\":{\"Id\":1,\"Capacity\":24,\"Amenities\":[0,1,2,3],\"WorkspaceType\":0,\"Coworking\":{\"Id\":1,\"Name\":\"WorkClub Pechersk\",\"Description\":\"Modern coworking in the heart of Pechersk with quiet rooms and coffee on tap.\",\"Address\":\"123 Yaroslaviv Val St, Kyiv\",\"Workspaces\":null},\"Bookings\":null},\"Seats\":1,\"StartDate\":\"2025-05-30\",\"EndDate\":\"2025-06-26\",\"StartTime\":\"12:00:00\",\"EndTime\":\"16:00:00\"},{\"Id\":2,\"Name\":\"John Snow\",\"Email\":\"user@example.com\",\"User\":null,\"Workspace\":{\"Id\":2,\"Capacity\":1,\"Amenities\":[2,3,5],\"WorkspaceType\":1,\"Coworking\":{\"Id\":1,\"Name\":\"WorkClub Pechersk\",\"Description\":\"Modern coworking in the heart of Pechersk with quiet rooms and coffee on tap.\",\"Address\":\"123 Yaroslaviv Val St, Kyiv\",\"Workspaces\":null},\"Bookings\":null},\"Seats\":1,\"StartDate\":\"2025-06-01\",\"EndDate\":\"2025-06-20\",\"StartTime\":\"08:00:00\",\"EndTime\":\"12:00:00\"},{\"Id\":3,\"Name\":\"John Snow\",\"Email\":\"user@example.com\",\"User\":null,\"Workspace\":{\"Id\":3,\"Capacity\":1,\"Amenities\":[2,3,5],\"WorkspaceType\":1,\"Coworking\":{\"Id\":2,\"Name\":\"UrbanSpace Podil\",\"Description\":\"A creative riverside hub ideal for freelancers and small startups.\",\"Address\":\"78 Naberezhno-Khreshchatytska St, Kyiv\",\"Workspaces\":null},\"Bookings\":null},\"Seats\":1,\"StartDate\":\"2025-05-01\",\"EndDate\":\"2025-05-10\",\"StartTime\":\"08:00:00\",\"EndTime\":\"12:00:00\"},{\"Id\":4,\"Name\":\"John Snow\",\"Email\":\"user@example.com\",\"User\":null,\"Workspace\":{\"Id\":4,\"Capacity\":20,\"Amenities\":[2,3,4,5],\"WorkspaceType\":2,\"Coworking\":{\"Id\":3,\"Name\":\"Creative Hub Lvivska\",\"Description\":\"A compact, design-focused space with open desks and strong community vibes.\",\"Address\":\"12 Lvivska Square, Kyiv\",\"Workspaces\":null},\"Bookings\":null},\"Seats\":20,\"StartDate\":\"2025-07-01\",\"EndDate\":\"2025-07-01\",\"StartTime\":\"08:00:00\",\"EndTime\":\"15:00:00\"},{\"Id\":5,\"Name\":\"John Snow\",\"Email\":\"user@example.com\",\"User\":null,\"Workspace\":{\"Id\":5,\"Capacity\":10,\"Amenities\":[2,3,4,5],\"WorkspaceType\":2,\"Coworking\":{\"Id\":4,\"Name\":\"TechNest Olimpiiska\",\"Description\":\"A high-tech space near Olimpiiska metro, perfect for team sprints and solo focus.\",\"Address\":\"45 Velyka Vasylkivska St, Kyiv\",\"Workspaces\":null},\"Bookings\":null},\"Seats\":10,\"StartDate\":\"2025-07-02\",\"EndDate\":\"2025-07-02\",\"StartTime\":\"10:00:00\",\"EndTime\":\"15:00:00\"}]");

            var response = await _coworkingDatabaseService.GetBookings("user@example.com");

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.NotNull(response.Data);
            response.Data.ForEach(booking => booking.Workspace.Coworking.Workspaces = null!);
            response.Data.ForEach(booking => booking.Workspace.Bookings = null!);
            response.Data.ForEach(booking => booking.User = null!);
            Assert.Equivalent(expected, response.Data);
        }

        [Fact]
        public async Task BookingsQuestion()
        {
            StringDto expected = new StringDto() { Value = "Sorry, I didn’t understand that. Please try rephrasing your question" };

            var response = await _coworkingDatabaseService.BookingsQuestion("user@example.com", new StringDto() { Value = "hello"});

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.NotNull(response.Data);
            Assert.Equivalent(expected, response.Data);
        }
    }
}
