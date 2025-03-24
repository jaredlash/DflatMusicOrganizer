using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace DflatCoreWPF.ViewModels;

public class ViewModelBase : ObservableObject
{
    public Action<bool?>? CloseAction { get; set; }


    public void TryClose()
    {
        CloseAction?.Invoke(true);
    }

    public void TryClose(bool? result)
    {
        CloseAction?.Invoke(result);
    }

    public virtual void OnClose()
    {

    }
}
