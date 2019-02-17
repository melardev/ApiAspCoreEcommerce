using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ApiCoreEcommerce.Entities
{
    [Serializable]
    public class ApplicationRole : IdentityRole<long>
    {
        public ApplicationRole(string name) : base(name)
        {
            CreationDate = DateTime.Now;
            ModificationDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override long Id { get; set; }

        public int VersionNumber { get; set; }
        public string Metadata { get; set; }
        public string Slug { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public long CreateBy { get; set; }
        public long ModifyBy { get; set; }
        public int Status { get; set; }

        public virtual ICollection<AppUserRole> Users { get; } = new List<AppUserRole>();
    }
}