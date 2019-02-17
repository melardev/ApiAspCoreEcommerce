namespace ApiCoreEcommerce.Dtos.Responses.Shared
{
    public class SuccessResponse : AppResponse
    {
        public SuccessResponse() : base(true)
        {
        }

        public SuccessResponse(string message) : base(true, message)
        {
        }
    }
}