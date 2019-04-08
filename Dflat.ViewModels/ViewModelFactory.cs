using Dflat.Business;
using Dflat.Business.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.ViewModels
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public ViewModelFactory(IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public FileSourceManagerViewModel CreateFileSourceManagerViewModel(IUnitOfWorkLifetimeManager uowLifetimeManager)
        {
            return new FileSourceManagerViewModel(uowLifetimeManager);
        }

        public T Create<T>() where T : ViewModelBase
        {
            //T viewModel;

            throw new NotImplementedException($"Creating {typeof(T)} instances not yet implemented");
            


            //return viewModel;
        }
    }
}
