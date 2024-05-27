using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ContactsApp.Avalonia.View.ViewModels;

namespace ContactsApp.Avalonia.View.Views
{
    public partial class ContactInfoView : ReactiveUserControl<ContactInfoViewModel>
    {
        public ContactInfoView()
        {
            InitializeComponent();
        }
    }
}
