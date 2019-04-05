using Dflat.Business.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business
{
    public class UnitOfWorkLifetimeManager : IDisposable, IUnitOfWorkLifetimeManager
    {
        private IUnitOfWork unitOfWork;
        private IUnitOfWorkFactory unitOfWorkFactory;

        private bool disposeUnitOfWork;

        public UnitOfWorkLifetimeManager(IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.unitOfWork = null;
            this.disposeUnitOfWork = false;
        }

        public UnitOfWorkLifetimeManager(IUnitOfWork unitOfWork) : this(unitOfWork, false)
        {
        }

        public UnitOfWorkLifetimeManager(IUnitOfWork unitOfWork, bool disposeUnitOfWork)
        {
            this.unitOfWorkFactory = null;
            this.unitOfWork = unitOfWork;

            this.disposeUnitOfWork = disposeUnitOfWork;
        }


        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (unitOfWork != null) return unitOfWork;

                if (unitOfWorkFactory == null)
                    throw new InvalidOperationException("Cannot create UnitOfWork with no UnitOfWorkFactory");

                unitOfWork = unitOfWorkFactory.Create();
                disposeUnitOfWork = true;

                return unitOfWork;
            }
        }

        public void Dispose()
        {
            if (disposeUnitOfWork)
                unitOfWork.Dispose();
        }
    }
}
