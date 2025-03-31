using DflatCoreWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using Unity;

namespace DflatCoreWPF.WindowService;

public class WindowService : IWindowService
{


    /// <summary>
    /// Maps a ViewModel type to an existing instance of a window view
    /// </summary>
    private readonly Dictionary<Type, Window> currentWindows;

    private readonly IUnityContainer iocContainer;


    public WindowService(IUnityContainer iocContainer)
    {
        this.iocContainer = iocContainer;

        currentWindows = [];
    }



    /// <summary>
    /// Shows a View/ViewModel that corresponds to an already registered TViewModel.  DataContext of the new view is set to the view model.
    /// </summary>
    /// <typeparam name="TViewModel">Type of the ViewModel for which this view is being created.</typeparam>
    /// <param name="viewModel">viewModel for which this is being created.</param>
    public void ShowWindow<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
    {
        Type vmType = typeof(TViewModel);

        if (currentWindows.TryGetValue(vmType, out Window? view) == false)
        {
            view = iocContainer.Resolve<Window>(vmType.Name);

            view.DataContext = viewModel;

            currentWindows.Add(vmType, view);

            viewModel.CloseAction = (r) => { view.Close(); };

            view.Closed += (o, args) =>
            {
                viewModel.OnClose();
                currentWindows.Remove(vmType);
                viewModel.CloseAction = null;
            };
        }

        view.Show();
        view.Activate();
    }

    /// <summary>
    /// Shows a View/ViewModel that corresponds to an already registered TViewModel.  DataContext of the new view is set to the view model.
    /// </summary>
    /// <typeparam name="TViewModel">Type of the ViewModel for which this view is being created.</typeparam>
    /// <param name="viewModel">viewModel for which this is being created.</param>
    public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
    {
        Window view;
        Type vmType = typeof(TViewModel);

        view = iocContainer.Resolve<Window>(vmType.Name);

        view.DataContext = viewModel;

        currentWindows.Add(vmType, view);

        viewModel.CloseAction = (r) => {
            view.DialogResult = r;
        };
        

        view.Closed += (o, args) =>
        {
            viewModel.OnClose();
            currentWindows.Remove(vmType);
            viewModel.CloseAction = null;
        };
        

        return view.ShowDialog();
    }
}
