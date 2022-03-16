using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Entities
{
    public interface IEntity<Tkey> where Tkey : struct
    {
        Tkey Id { get; }
        Guid Guid { get; }  // 128 bit
    }
}
