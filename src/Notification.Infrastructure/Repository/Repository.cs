using Microsoft.EntityFrameworkCore;
using Notification.Domain;
using Notification.Domain.Entities;
using Notification.Infrastructure.Context;
using Notification.Infrastructure.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {

        protected readonly NotificationContext _context;
        private readonly DbSet<T> _dataSet;

        public Repository(NotificationContext context)
        {
            _context = context;
            _dataSet = _context.Set<T>();
        }

        public async Task<bool> DeleteAsync(int Id)
        {

            var result = await _dataSet.SingleOrDefaultAsync(c => c.Id.Equals(Id));

            if (result == null)
                return false;

            _dataSet.Remove(result);

            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<T> InsertAsync(T item)
        {
            try
            {
                item.CreateAt = DomainUtils.GetLocalDate();
                _dataSet.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ExistAsync(int Id)
        {
            return await _dataSet.AllAsync(p => p.Id.Equals(Id));
        }

        public async Task<IEnumerable<T>> SelectAllAsync()
        {
            return await _dataSet.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<T> SelectAsync(int Id)
        {
            return await _dataSet.SingleOrDefaultAsync(c => c.Id.Equals(Id));
        }

        public async Task<T> UpdateAsync(T item)
        {
            var result = await _dataSet.SingleOrDefaultAsync(p => p.Id.Equals(item.Id));

            if (result == null)
                return null;

            item.CreateAt = result.CreateAt;

            _context.Entry(result).State = EntityState.Modified;
            _context.Entry(result).CurrentValues.SetValues(item);           

            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<T> Delete(T item)
        {
            var result = await _dataSet.SingleOrDefaultAsync(p => p.Id.Equals(item.Id));

            if (result == null)
                return null;

            item.CreateAt = result.CreateAt;
            item.Active = null;
            _context.Entry(result).CurrentValues.SetValues(item);

            await _context.SaveChangesAsync();

            return item;
        }
    }
}
