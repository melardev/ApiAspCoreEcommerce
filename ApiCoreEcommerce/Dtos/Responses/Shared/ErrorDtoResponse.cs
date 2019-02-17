namespace ApiCoreEcommerce.Dtos.Responses.Shared
{
    public class ErrorDtoResponse : AppResponse
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public ErrorDtoResponse() : base(false)
        {
        }

        public ErrorDtoResponse(string message) : base(false, message)
        {
        }
    }
}