using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SPG.AirQuality.Models
{
    public partial class Stations
    {
        public Stations()
        {
            Measurements = new HashSet<Measurements>();
        }

        [Key]
        public string Id { get; set; }
        public long ObjectId { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Street { get; set; }
        public string Topo { get; set; }
        public string Usage { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public long CoordinateX { get; set; }
        public long CoordinateY { get; set; }
        public long Height { get; set; }
        public string Notice { get; set; }

        [InverseProperty("Station")]
        public virtual ICollection<Measurements> Measurements { get; set; }
    }
}
