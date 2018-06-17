using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BoilerController.Api.Contracts;

namespace BoilerController.Api.Models.Devices
{
    public enum DeviceType
    {
        Input,
        Output,
        PWM
    }
    public class Device : IEntity
    {
        [Key]
        [Column("DeviceId")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "You must specify device type")]
        [Column("DeviceType")]
        public DeviceType Type { get; }
        [Required(ErrorMessage = "You must specify device name")]
        [Column("DeviceName")]
        public string DeviceName { get; set; }
        [Required(ErrorMessage = "You must specify device pin")]
        [Column("DevicePin")]
        public int DevicePin { get; set; }
        public bool State { get; set; }
    }
}
