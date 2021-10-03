using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerCovid19.Models
{
    public class Member
    {
        [Key]
        public string MemberRegistrationNumber { get; set; }
        public string Name { get; set; }
        public string Sector { get; set; }
        public string Password { get; set; }
        [Column(TypeName = "Date")]
        [Display(Name = "Data de nascimento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public bool Admin { get; set; }
        public virtual ICollection<HealthRegistration> HealthRegistrations { get; set; }
    }
}
