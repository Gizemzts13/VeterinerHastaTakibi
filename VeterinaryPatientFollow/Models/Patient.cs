using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VeterinaryPatientFollow.Models
{
    public class Patient 
    {
        [Key]
        public int PatientID { get; set; }
        [Column("nvarchar(250")]
        [Required(ErrorMessage ="Lütfen bu alanı doldurunuz !")]
        [DisplayName("Hasta Adı")]
        public string Name { get; set; }
        [DisplayName("Hasta Fotoğrafı")]
        public string ImageURL { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        [DisplayName("Sahip Bilgisi")]
        public string Owner { get; set; }
        [DisplayName("Cinsi")]
        public string Genus { get; set; }
        [DisplayName("Tanı")]
        public string Diagnosis { get; set; }
    }
}
