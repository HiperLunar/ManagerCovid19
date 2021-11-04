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
        public enum Sectors
        {
            Administrador,
            Aluno,
            Professor,
            Funcionario            
        }

        [Key]
        [Display(Name = "Número de matrícula")]
        public string MemberRegistrationNumber { get; set; }
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Display(Name = "Setor")]
        public Sectors Sector { get; set; }
        [Display(Name = "Senha")]
        public string Password { get; set; }
        [Column(TypeName = "Date")]
        [Display(Name = "Data de nascimento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [Display(Name = "Cidade")]
        public string City { get; set; }
        [Display(Name = "Estado")]
        public string State { get; set; }
        [Display(Name = "Registros de saúde")]
        public virtual ICollection<HealthRegistration> HealthRegistrations { get; set; }
    }
}
