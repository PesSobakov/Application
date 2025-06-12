using Application.Server.Models.DTOs;
using Application.Server.Models.CoworkingDatabase;
using Application.Server.Models.DTOs.GetCoworkings;
using Application.Server.Models.DTOs.GetWorkspaces;

namespace Application.Server.Services
{
    public interface IGroqService
    {
        public Task<ServiceResponse<string>> MakeRequest(string systemPrompt, string assistantPrompt, string userPrompt);
    }
}
