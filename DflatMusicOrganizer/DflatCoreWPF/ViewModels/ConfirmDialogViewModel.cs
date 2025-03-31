using CommunityToolkit.Mvvm.Input;

namespace DflatCoreWPF.ViewModels;

public partial class ConfirmDialogViewModel : ViewModelBase
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string YesText { get; set; } = string.Empty;
    public string NoText { get; set; } = string.Empty;

    [RelayCommand]
    public void Yes()
    {
        TryClose(true);
    }

    [RelayCommand]
    public void No()
    {
        TryClose(false);
    }
}
