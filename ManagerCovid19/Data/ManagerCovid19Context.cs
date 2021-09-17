using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ManagerCovid19.Models;

namespace ManagerCovid19.Data
{
    public class ManagerCovid19Context : DbContext
    {
        public ManagerCovid19Context (DbContextOptions<ManagerCovid19Context> options)
            : base(options)
        {
        }

        public DbSet<ManagerCovid19.Models.Member> Member { get; set; }

        public DbSet<ManagerCovid19.Models.HealthRegistration> HealthRegistration { get; set; }
    }
}
