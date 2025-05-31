namespace Application.Server.Services
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public ResponseStatus Status { get; set; }
        public static implicit operator ServiceResponse<T>(ServiceResponse serviceResponse) => new ServiceResponse<T>() { Status = serviceResponse.Status };
    }
    public class ServiceResponse
    {
        public ResponseStatus Status { get; set; }
        public List<string> ErrorMessages { get; set; } = new();

    }
}
