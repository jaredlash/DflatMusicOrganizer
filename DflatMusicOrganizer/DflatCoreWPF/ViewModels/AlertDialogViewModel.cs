﻿using CommunityToolkit.Mvvm.Input;

namespace DflatCoreWPF.ViewModels;

public partial class AlertDialogViewModel : ViewModelBase
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    [RelayCommand]
    public void Okay()
    {
        TryClose(true);
    }
}
