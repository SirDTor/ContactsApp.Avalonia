using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using ContactsApp.Avalonia.View.ViewModels;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ContactsApp.Avalonia.View.Views;

public partial class ContactsListView : ReactiveUserControl<MainViewModel>
{
    public ContactsListView()
    {
        InitializeComponent();
    }
}
