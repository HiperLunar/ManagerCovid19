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
        [Display(Name = "Data e hora do registro")]
        public DateTime RegisterDateTime { get; set; }
        [Display(Name = "Está de sentindo bem?")]
        public bool HowRUFeeling { get; set; }
        [Display(Name = "Falta de ar")]
        public bool FaltaDeAr { get; set; }
        [Display(Name = "Cansaço")]
        public bool Cansaco { get; set; }
        [Display(Name = "Febre")]
        public bool Febre { get; set; }
        [Display(Name = "Calafrios")]
        public bool Calafrios { get; set; }
        [Display(Name = "Tosse")]
        public bool Tosse { get; set; }
        [Display(Name = "Dor de garganta")]
        public bool DorDeGarganta { get; set; }
        [Display(Name = "Dor de cabeça")]
        public bool DorDeCabeca { get; set; }
        [Display(Name = "Dor no peito")]
        public bool DorNoPeito { get; set; }
        [Display(Name = "Perda de olfato")]
        public bool PerdaDeOlfato { get; set; }
        [Display(Name = "Perda de paladar")]
        public bool PerdaPaladar { get; set; }
        [Display(Name = "Diarreia")]
        public bool Diarreia { get; set; }
        [Display(Name = "Coriza")]
        public bool Coriza { get; set; }
        [Display(Name = "Espirros")]
        public bool Espirros { get; set; }
        public virtual Member Member { get; set; }
    }
}
