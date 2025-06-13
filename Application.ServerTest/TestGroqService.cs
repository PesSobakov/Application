using Application.Server.Services;

namespace TaskBoard.ServerTest
{
    internal class TestGroqService : IGroqService
    {
       ServiceResponse<string> response = new ServiceResponse<string>() { Data = "{\"type\": \"unknown\"}", Status = ResponseStatus.Ok};
        public async Task<ServiceResponse<string>> MakeRequest(string systemPrompt, string inputData, string userPrompt)
        {
            return response;
        }
        public void SetResponse(ServiceResponse<string> response)
        {
            this.response = response;
        }

    }
}
