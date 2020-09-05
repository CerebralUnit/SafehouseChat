using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Safehouse.Repository.Interfaces
{
    public interface IRelationshipRepository<T>
    {
        public Task<T> Retrieve(string key1, string key2); 
        public Task<bool> Exists(string key1, string key2);
        public Task<bool> Update(T obj);
        public Task<string> Create(T obj);
        public Task<bool> Delete(T obj);
    }
}
