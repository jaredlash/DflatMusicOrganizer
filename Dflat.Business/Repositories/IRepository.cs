using System.Collections.Generic;

namespace Dflat.Business.Repositories
{
    public interface IRepository<T>
    {
        T Get(int id);
        List<T> GetAll();

        T Create();

        void Remove(T item);
        void Remove(int id);
    }
}
