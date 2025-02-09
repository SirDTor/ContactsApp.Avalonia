using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using ContactsApp.Avalonia.View.ViewModels;
using ReactiveUI;
using System;

namespace ContactsApp.Avalonia.View.Views
{
    public partial class EditContactView : ReactiveWindow<EditContactViewModel>
    {
        public EditContactView()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel.EditContactCommand.Subscribe(Close)));
            this.BindCommand(this.ViewModel, vm => vm.EditContactCommand, v => v.EditContactButton);           

            HotKeyManager.SetHotKey(CancelEditButton, new KeyGesture(Key.Escape, KeyModifiers.None));
        }

        private void CancelEditButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
