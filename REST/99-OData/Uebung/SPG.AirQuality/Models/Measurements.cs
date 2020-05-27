using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SPG.AirQuality.Models
{
    public partial class Measurements
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [Column(TypeName = "DATETIME")]
        public DateTime MeasurementDate { get; set; }
        public double? Temperature { get; set; }
        public double? WindVelocity { get; set; }
        public long? WindDirection { get; set; }
        public long? NO2 { get; set; }
        public long? NO { get; set; }
        public long? NOX { get; set; }
        public long? PM10 { get; set; }
        public long? PM25 { get; set; }
        public long? Humidity { get; set; }
        public long? LDR { get; set; }
        public long? STR { get; set; }
        [Required]
        public string StationId { get; set; }

        [ForeignKey(nameof(StationId))]
        [InverseProperty(nameof(Stations.Measurements))]
        public virtual Stations Station { get; set; }
    }
}
