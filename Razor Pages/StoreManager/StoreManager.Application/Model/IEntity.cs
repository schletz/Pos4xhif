using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Application.Model
{
    public interface IEntity<T> where T : struct
    {
        T Id { get; }
        Guid Guid { get; }
    }
}
