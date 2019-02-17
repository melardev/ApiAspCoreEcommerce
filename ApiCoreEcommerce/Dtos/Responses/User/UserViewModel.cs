using System;
using System.ComponentModel.DataAnnotations;

namespace ApiCoreEcommerce.Dtos.Responses.User
{
  public class ProfileViewModel
  {
    [Required]
    public long Id { get; set; }

    [Required]
    public string UserName {get; set;}
    
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

  [Required]
  public DateTime CreatedAt {get;set;}

    [Required]
    public DateTime UpdatedAt {get;set;}
    
    public int CommentsCount {get;set;}
  }
}
