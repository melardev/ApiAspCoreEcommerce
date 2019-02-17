using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogDotNet.Models.ViewModels.User
{
    public class CreateOrEditUsertViewModel
    {
        public long Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
    }
}
