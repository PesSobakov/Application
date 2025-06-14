using Application.Server.Models.DTOs;
using Application.Server.Models.CoworkingDatabase;
using Application.Server.Models.DTOs.GetCoworkings;
using Application.Server.Models.DTOs.GetWorkspaces;

namespace Application.Server.Services
{
    public interface ICoworkingDatabaseService
    {
        public Task<ServiceResponse<List<CoworkingDto>>> GetCoworkings();
        public Task<ServiceResponse<List<WorkspaceGroupDto>>> GetWorkspaces(string? login, int id);
        public Task<ServiceResponse<User>> Login(LoginDto loginDto);
        public Task<ServiceResponse<User>> GetUser(string login);
        public Task<ServiceResponse> CreateBooking(string login, CreateBookingDto createBookingDto);
        public Task<ServiceResponse> DeleteBooking(string login, int id);
        public Task<ServiceResponse> EditBooking(string login, int id,EditBookingDto editBookingDto);
        public Task<ServiceResponse<Booking>> GetBooking(string login, int id);
        public Task<ServiceResponse<List<Booking>>> GetBookings(string login);
        public Task<ServiceResponse<StringDto>> BookingsQuestion(string login, StringDto question);


        public Task<ServiceResponse> Seed();
    }
}
