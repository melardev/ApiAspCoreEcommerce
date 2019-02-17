using Newtonsoft.Json;

namespace BlogDotNet.Models.ViewModels.Requests.Comment
{
    public class CreateOrEditCommentDto
    {
        public string Content { get; set; }
        public int? Rating { get; set; }
    }
}