using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Model;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Application.Infrastructure.Repositories
{
    /// <summary>
    /// Basisklasse für alle Repositories.
    /// </summary>
    public abstract class Repository<Tentity, Tkey> where Tentity : class, IEntity<Tkey> where Tkey : struct
    {
        protected readonly StoreContext _db;
        public IQueryable<Tentity> Set => _db.Set<Tentity>();
        public Repository(StoreContext db)
        {
            _db = db;
        }

        public Tentity? FindById(Tkey id) => _db.Set<Tentity>().FirstOrDefault(e => e.Id.Equals(id));
        public Tentity? FindByGuid(Guid guid) => _db.Set<Tentity>().FirstOrDefault(e => e.Guid == guid);
        public virtual (bool success, string message) Insert(Tentity entity)
        {
            _db.Entry(entity).State = EntityState.Added;
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
            return (true, string.Empty);
        }
        public virtual (bool success, string message) Update(Tentity entity)
        {
            if (!HasPrimaryKey(entity)) { return (false, "Missing primary key."); }

            _db.Entry(entity).State = EntityState.Modified;
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
            return (true, string.Empty);
        }
        public virtual (bool success, string message) Delete(Tentity entity)
        {
            if (!HasPrimaryKey(entity)) { return (false, "Missing primary key."); }

            _db.Entry(entity).State = EntityState.Deleted;
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return (false, ex.Message);
            }
            return (true, string.Empty);
        }

        public virtual (bool success, string message) DeleteByGuid(Guid guid)
        {
            var entity = _db.Set<Tentity>().FirstOrDefault(e => e.Guid == guid);
            if (entity is null) { return (true, string.Empty); }
            return Delete(entity);
        }
        private bool HasPrimaryKey(Tentity entity) => !entity.Id.Equals(default);
    }
}
