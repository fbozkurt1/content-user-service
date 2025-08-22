using System.Text.Json.Serialization;

namespace UserService.API.Infrastructure
{
    public class BaseResponse
    {
        [JsonPropertyOrder(1)]
        public string? Message { get; set; }

        public BaseResponse(string? message)
        {
            Message = message;
        }
    }
    public class BaseResponse<T> : BaseResponse
    {
        [JsonPropertyOrder(2)]
        public T? Result { get; set; }

        public BaseResponse(string? message, T? result) : base(message)
        {
            Result = result;
        }

        public BaseResponse(T? result) : base(null)
        {
            Result = result;
        }
    }
}
