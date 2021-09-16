using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerCovid19.Models
{
    public class HealthRegistration
    {
        [Key]
        public int HealthRegistrationID { get; set; }
        [ForeignKey("Member")]
        public string MemberRegistrationNumber { get; set; }
        public DateTime RegisterDateTime { get; set; }
        public bool HowRUFeeling { get; set; }
        public Member Member { get; set; }
    }
}
