using Billing.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Billing.Models.Models
{
    public class Invoice
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        [JsonIgnore]
        public Customer Customer { get; set; }

        [NotMapped]
        public string CustomerName
        {
            get
            {
                return this.Customer != null ? this.Customer.FirstName + " " + this.Customer.LastName : "";
            }
            set { }
        }

        [Required(ErrorMessage = "The amount field is required.")]
        //[Range(0.1, double.MaxValue, ErrorMessage = "This field must be greater than 0.")]
        public double Amount { get; set; }

        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        //[Column(TypeName = "Date")]
        public DateTime DeadLine { get; set; }
        [JsonIgnore]
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        [JsonIgnore]
        public StatusEnum Status { get; set; } = StatusEnum.Approved;
    }
}
