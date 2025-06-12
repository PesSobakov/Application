using Microsoft.EntityFrameworkCore;
using Application.Server.Models.DTOs;
using Application.Server.Models.CoworkingDatabase;
using Application.Server.Models.DTOs.GetCoworkings;
using System.Collections.Generic;
using Application.Server.Models.DTOs.GetWorkspaces;
using Microsoft.Extensions.Configuration;
using GroqSharp.Models;
using GroqSharp;

namespace Application.Server.Services
{
    public class GroqService : IGroqService
    {
        private readonly IConfiguration _configuration;
        private readonly ITimeProvider _timeProvider;
        private readonly GroqClient _groqClient;
        public GroqService(CoworkingContext context, ITimeProvider timeProvider, IConfiguration configuration)
        {
            _configuration = configuration;
            _timeProvider = timeProvider;

            string apiKey = _configuration["GROQ_API_KEY"]!;
            string apiModel = "llama3-70b-8192";
            //string apiModel = "llama-3.3-70b-versatile";
            //string apiModel = "meta-llama/llama-4-maverick-17b-128e-instruct";
            //string apiModel = "meta-llama/llama-4-scout-17b-16e-instruct";

            _groqClient = new GroqClient(apiKey, apiModel);
             _groqClient.SetTemperature(0);
             _groqClient.SetStop("```");
        }

        public async Task<ServiceResponse<string>> MakeRequest(string systemPrompt, string inputData, string userPrompt)
        {
            try
            {
                var response = await _groqClient.CreateChatCompletionAsync(
                    new Message { Role = MessageRoleType.System, Content = systemPrompt },
                    new Message { Role = MessageRoleType.System, Content = inputData },
                    new Message { Role = MessageRoleType.User, Content = userPrompt },
                    new Message { Role = MessageRoleType.Assistant, Content= "```json" }
                );

                return new ServiceResponse<string>
                {
                    Status = ResponseStatus.Ok,
                    Data = response
                };
            }
            catch (Exception)
            {
                return new ServiceResponse { Status = ResponseStatus.InternalServerError };
            }

        }
    }
}
