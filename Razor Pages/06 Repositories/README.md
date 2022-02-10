# Razor Pages 6 - Repository Pattern

## Inhalt

- Basisinterface für Entities
- Alternate Key für GUID Werte
- Das Repository Pattern
- Generisches Repository

Das Video ist auf https://youtu.be/FgnntpuKGuQ verfügbar (60min). Der Programmcode ist im
Ordner [StoreManager](StoreManager) zu finden.

Voraussetzung ist der Inhalt des vorigen Kapitels [05 Bulk Edit](../05%20Bulk%20Edit/README.md)

> **Hinweis:** Die DTO Klassen werden in diesem Beispiel im Webapp Projekt angelegt, da sie
> auch Validierungen beinhalten. Sie können diese Klassen auch im Application Projekt anlegen,
> dann kann z. B. die Insert Methode des Repositories die DTO Klasse als Parameter haben. Dann
> ersparen Sie sich das Definieren der Properties als Funktionsargumente.


![](https://codewithmukesh.com/wp-content/uploads/2020/06/custom-repo-versus-db-context-1024x579.png?ezimgfmt=ng:webp/ngcb32)
<sup>
Quelle: https://codewithmukesh.com/blog/repository-pattern-in-aspnet-core/
</sup>

**StoreManager.Application/Infrastructure/Repositories/Repository.cs**
```c#
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

        private bool HasPrimaryKey(Tentity entity) => !entity.Id.Equals(default);
    }
}
```