using Billing.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Billing.Models.Models
{
    public class Customer
    {
        public Customer()
        {
            InvoiceList = new List<Invoice>();
        }

        [Key]
        public Guid Id { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(50, ErrorMessage = "Max alowed length is 50 characters.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(50, ErrorMessage = "Max alowed length is 50 characters.")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Email address should be like abc@example.com")]
        public string Email { get; set; } = null;

        public string ImgUrl { get; set; } = null;

        [JsonIgnore]
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        [JsonIgnore]
        public StatusEnum Status { get; set; } = StatusEnum.Approved;
        [JsonIgnore]
        public List<Invoice> InvoiceList { get; set; } = new List<Invoice>();
    }
}
