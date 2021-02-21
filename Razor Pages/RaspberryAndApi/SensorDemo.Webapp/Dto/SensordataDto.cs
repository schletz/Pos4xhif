using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorDemo.Webapp.Dto
{
    public record SensordataDto(
        decimal Temperature,
        decimal Timestamp
    );
}
