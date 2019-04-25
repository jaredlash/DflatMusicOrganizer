using Dflat.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Repositories
{
    public interface IFileRepository :IRepository<File>
    {

        bool Contains(File file);
    }
}
