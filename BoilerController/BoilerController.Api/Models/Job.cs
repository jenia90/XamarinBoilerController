using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BoilerController.Api.Models
{
    [Table("Job")]
    public class Job
    {
        [Key]
        [Column("JobId")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "You must specify end time")]
        [Column("End")]
        public string End { get; set; }

        [Required(ErrorMessage = "You must specify device pin")]
        [Column("Pin")]
        public int Pin { get; set; }

        [Column("DeviceName")]
        public string DeviceName { get; set; }

        [Required(ErrorMessage = "You must specify start time")]
        [Column("Start")]
        public string Start { get; set; }

        [Column("Type")]
        public string Type { get; set; }

        [Column("DayList")]
        public IEnumerable<string> DaysList { get; set; }
    }
}
