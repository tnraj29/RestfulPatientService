using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestfulPatientService.Repository
{
    // This is a Generic Repository API for Persisting all entity types.
    public interface IGenericRepository<T> : IDisposable where T : class
    {
        IQueryable<T> GetAll();

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        void Add(T entity);

        void Remove(T entity);

        void Edit(T entity);

        Task SaveAsync();
    }
}