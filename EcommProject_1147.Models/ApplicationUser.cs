using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommProject_1147.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Display(Name ="StreetAddress")]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        [Display(Name ="Company")]
        public int ?CompanyId { get; set; }
        [ForeignKey ("CompanyId")]
        public Company company { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}
