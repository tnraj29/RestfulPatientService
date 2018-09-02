using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace RestfulPatientService.Repository
{
    // This is a Generic Abstract Repositoy class that implements the "IGenericRepository" interface, 
    // it takes Entity type and Database context type as parameters to enable reuse of generic CRUD operations 
    // on all Entity types and Database context types.
    public abstract class GenericRepository<T, DBC>: IGenericRepository<T> where T: class where DBC: DbContext, new()
    {
        private DBC _dbContext = new DBC();

        protected DBC Context
        {
            get { return _dbContext; }
            set { _dbContext = value; }
        }

        public virtual IQueryable<T> GetAll()
        {

            IQueryable<T> query = _dbContext.Set<T>();
            return query;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _dbContext.Set<T>().Where(predicate);
            return query;
        }

        public virtual void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public virtual void Remove(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public virtual void Edit(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual async Task SaveAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbUpdateConcurrencyException(e.Message);
            }
            catch (DbUpdateException e)
            {
                throw new DbUpdateException(e.Message);
            }
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
                if (disposing)
                    _dbContext.Dispose();

            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
        }
    }
}