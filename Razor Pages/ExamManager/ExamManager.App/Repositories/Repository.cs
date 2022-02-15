using ExamManager.App.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Repositories
{
    public abstract class Repository<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : struct
    {
        protected readonly ExamContext _db;

        public Repository(ExamContext db)
        {
            _db = db;
        }

        public IQueryable<TEntity> Set => _db.Set<TEntity>();
        public virtual (bool success, string message) Insert(TEntity exam)
        {
            _db.Entry(exam).State = EntityState.Added;
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return (false, e.InnerException?.Message ?? e.Message);
            }
            return (true, string.Empty);

        }
        public virtual (bool success, string message) Update(TEntity exam)
        {
            if (exam.Id.Equals(default)) { return (false, "Missing primary key."); }
            if (exam.Guid.Equals(default)) { return (false, "Missing primary key."); }
            _db.Entry(exam).State = EntityState.Modified;
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return (false, e.InnerException?.Message ?? e.Message);
            }
            return (true, string.Empty);

        }
        public virtual (bool success, string message) Delete(TEntity exam)
        {
            if (exam.Id.Equals(default)) { return (false, "Missing primary key."); }
            _db.Entry(exam).State = EntityState.Deleted;
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return (false, e.InnerException?.Message ?? e.Message);
            }
            return (true, string.Empty);

        }
    }
}
