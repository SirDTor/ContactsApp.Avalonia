using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using ContactsApp.Avalonia.View.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;

namespace ContactsApp.Avalonia.View.Views;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        var aboutWindow = this.FindControl<Button>("AboutWindowButton");        

        HotKeyManager.SetHotKey(aboutWindow, new KeyGesture(Key.F1, KeyModifiers.None));        

        this.BindCommand(this.ViewModel, vm => vm.AboutCommand, v => v.AboutWindowButton);
        this.WhenActivated(d =>
            d(ViewModel.ShowDialog.RegisterHandler(i => ShowDialog(i, new AboutView()))));
        this.WhenActivated(d =>
            d(ViewModel.AddContactWindow.RegisterHandler(i => ContactWork(i, new AddContactView()))));
        this.WhenActivated(d =>
            d(ViewModel.EditContactWindow.RegisterHandler(i => ContactWork(i, new EditContactView()))));
        this.WhenActivated(d =>
            d(ViewModel.ConfirmationWindow.RegisterHandler(i => ShowDialog(i, new ConfirmationView()))));
    }

    /// <summary>
    /// Метод открытия диалогового окна
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="interaction"></param>
    /// <param name="dialog"></param>
    /// <returns></returns>
    private async Task ShowDialog<TViewModel, T>(IInteractionContext<TViewModel, T> interaction, ReactiveWindow<TViewModel> dialog) where TViewModel : class
    {
        dialog.DataContext = interaction.Input;

        var result = await dialog.ShowDialog<T>(this);
        interaction.SetOutput(result);
    }

    private async Task ContactWork<TViewModel, T>(IInteractionContext<TViewModel, T> interaction, ReactiveWindow<TViewModel> dialog) where TViewModel : class
    {
        dialog.DataContext = interaction.Input;
        var result = await dialog.ShowDialog<T>(this);
        interaction.SetOutput(result);
    }
}
