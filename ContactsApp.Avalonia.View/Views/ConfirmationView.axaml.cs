using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ContactsApp.Avalonia.View.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ContactsApp.Avalonia.View
{
    public partial class ConfirmationView : ReactiveWindow<ConfirmationViewModel>
    {
        public ConfirmationView()
        {
            InitializeComponent();
            this.BindCommand(this.ViewModel, vm => vm.DeleteContactCommand, v => v.YesButton);
            this.BindCommand(this.ViewModel, vm => vm.CancelCommand, v => v.NoButton);
            this.WhenActivated(disposables =>
            {
                if (ViewModel != null)
                {
                    ViewModel.WhenAnyValue(vm => vm.WindowResult)
                             .Where(result => result.HasValue) // Фильтрация значений
                             .Subscribe(result =>
                             {
                                 Close(result.Value); // Закрыть окно с результатом
                             })
                             .DisposeWith(disposables);
                }
            });
        }
    }
}