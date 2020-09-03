using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Safehouse.Repository.Interfaces
{
    public interface IRepository<T>
    {
        public Task<T> Retrieve(string key);
        public Task<List<T>> RetrieveMany(List<string> keys);
        public Task<bool> Update(T obj);
        public Task<string> Create(T obj);
        public Task<bool> Delete(string key);
    }
}
