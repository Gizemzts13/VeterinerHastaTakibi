using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VeterinaryPatientFollow.Models
{
    public class PatientRegistrationContext :DbContext
    {
        public PatientRegistrationContext(DbContextOptions<PatientRegistrationContext> options) : base(options)
        {
                
        }
        public DbSet<Patient> Patients { get; set; }
    }
}
