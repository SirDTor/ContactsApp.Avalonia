using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using ContactsApp.Avalonia.View.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace ContactsApp.Avalonia.View.Views
{
    public partial class EditContactView : ReactiveWindow<EditContactViewModel>
    {
        public EditContactView()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.BindCommand(ViewModel, vm => vm.EditContactCommand, v => v.EditContactButton)
                    .DisposeWith(disposables);

                ViewModel!.EditContactCommand.Subscribe(result =>
                {
                    if (result != null)
                        Close(result);
                }).DisposeWith(disposables);
            });  
            HotKeyManager.SetHotKey(CancelEditButton, new KeyGesture(Key.Escape, KeyModifiers.None));
        }

        private void CancelEditButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
