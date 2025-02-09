using ContactsApp.Model;
using ReactiveUI;
using System.Reactive;

namespace ContactsApp.Avalonia.View.ViewModels
{
    public class ConfirmationViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> DeleteContactCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        private bool? _windowResult;
        public bool? WindowResult
        {
            get => _windowResult;
            set => this.RaiseAndSetIfChanged(ref _windowResult, value);
        }

        public ConfirmationViewModel(Contact contact)
        {
            DeleteContactCommand = ReactiveCommand.Create(() =>
            {
                WindowResult = true;
            });

            CancelCommand = ReactiveCommand.Create(() =>
            {
                WindowResult = false;
            });
        }
    }
}