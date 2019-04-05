using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Factories
{
    public class UowLifetimeManagerFactory : IFactory<IUnitOfWorkLifetimeManager>, IUowLifetimeManagerFactory
    {
        private readonly IUnitOfWorkFactory uowFactory;

        public UowLifetimeManagerFactory(IUnitOfWorkFactory uowFactory)
        {
            this.uowFactory = uowFactory;
        }

        public IUnitOfWorkLifetimeManager Create()
        {
            return new UnitOfWorkLifetimeManager(uowFactory);
        }

        public IUnitOfWorkLifetimeManager Create(IUnitOfWork unitOfWork)
        {
            return new UnitOfWorkLifetimeManager(unitOfWork);
        }
    }
}
