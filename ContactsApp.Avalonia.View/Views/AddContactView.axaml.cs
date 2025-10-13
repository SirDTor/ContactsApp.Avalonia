using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using ContactsApp.Avalonia.View.ViewModels;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace ContactsApp.Avalonia.View.Views
{
    public partial class AddContactView : ReactiveWindow<AddContactViewModel>
    {
        public AddContactView()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.BindCommand(ViewModel, vm => vm.AddContactCommand, v=>v.AddContactButton)
                    .DisposeWith(disposables);
                ViewModel!.AddContactCommand.Subscribe(result =>
                {
                    if (result != null) Close(result);
                }).DisposeWith(disposables);
            });

            HotKeyManager.SetHotKey(CancelButton, new KeyGesture(Key.Escape, KeyModifiers.None));
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
