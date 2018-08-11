using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BoilerController.Api.Contracts;

namespace BoilerController.Api.Models
{
    public enum DeviceType
    {
        Input = 0,
        Output = 1,
        PWM = 2
    }
    public class Device : IEntity
    {
        [Key]
        [Column("DeviceId")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "You must specify device type")]
        [Column("DeviceType")]
        public DeviceType Type { get; set; }
        [Required(ErrorMessage = "You must specify device name")]
        [Column("DeviceName")]
        public string DeviceName { get; set; }
        [Required(ErrorMessage = "You must specify device pin")]
        [Column("DevicePin")]
        public int DevicePin { get; set; }
        public bool State { get; set; }
        [Column("Jobs")]
        public IEnumerable<Job> Jobs { get; set; }
    }
}
