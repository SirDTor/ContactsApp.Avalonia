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

        var addContact = this.FindControl<Button>("AddContactButton");
        var editContact = this.FindControl<Button>("EditContactButton");
        var deleteContact = this.FindControl<Button>("DeleteContactButton");

        HotKeyManager.SetHotKey(addContact, new KeyGesture(Key.I, KeyModifiers.Control));
        HotKeyManager.SetHotKey(editContact, new KeyGesture(Key.F2, KeyModifiers.None));
        HotKeyManager.SetHotKey(deleteContact, new KeyGesture(Key.Delete, KeyModifiers.None));
    }
}
