using Notification.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Notification.Infrastructure.Repository.Interface
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> InsertAsync(T item);
        Task<T> UpdateAsync(T item);
        Task<bool> DeleteAsync(int Id);
        Task<IEnumerable<T>> SelectAllAsync();
        Task<T> SelectAsync(int Id);
        Task<T> Delete(T item);

    }
}
