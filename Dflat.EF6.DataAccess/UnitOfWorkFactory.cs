using Dflat.Business.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dflat.Business;

namespace Dflat.EF6.DataAccess
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork Create()
        {
            return new UnitOfWork();
        }
    }
}
