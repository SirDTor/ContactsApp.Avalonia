using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using ContactsApp.Avalonia.View.ViewModels;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;

namespace ContactsApp.Avalonia.View.Views
{
    public partial class AddContactView : ReactiveWindow<AddContactViewModel>
    {
        public AddContactView()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel.AddContactCommand.Subscribe(Close)));
            this.BindCommand(this.ViewModel, vm => vm.AddContactCommand, v => v.AddContactButton);
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
