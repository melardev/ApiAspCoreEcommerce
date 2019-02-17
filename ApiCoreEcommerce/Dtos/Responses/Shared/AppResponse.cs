using System.Collections.Generic;

namespace ApiCoreEcommerce.Dtos.Responses.Shared
{
    public class AppResponse
    {
        public AppResponse(bool success)
        {
            Success = success;
        }

        protected AppResponse(bool success, string message) : this(success)
        {
            if (FullMessages == null)
                FullMessages = new List<string>();
            FullMessages.Add(message);
        }

        public bool Success { get; set; }
        public ICollection<string> FullMessages { get; set; } = new List<string>();
    }
}