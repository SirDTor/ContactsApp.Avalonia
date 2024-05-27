using ContactsApp.Model;
using DynamicData;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Reactive.Linq;
using System.Reactive;
using Avalonia.Media.Imaging;
using System.Windows.Input;
using System.Linq;

namespace ContactsApp.Avalonia.View.ViewModels;

public class MainViewModel : ViewModelBase
{
    /// <summary>
    /// 
    /// </summary>
    private Contact _selectedContact;

    /// <summary>
    /// 
    /// </summary>
    private Contact _currentContact = new Contact();

    /// <summary>
    /// 
    /// </summary>
    public ReactiveCommand<Unit, Unit> GenerateRandomContactsCommand { get; }

    /// <summary>
    /// 
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddContactCommand { get; }

    /// <summary>
    /// Команда открытия окна "О программе"
    /// </summary>
    public ReactiveCommand<Unit, Unit> AboutCommand { get; }

    /// <summary>
    /// 
    /// </summary>
    public Project Project { get; set; } = new Project();

    /// <summary>
    /// 
    /// </summary>
    public ObservableCollection<Contact> Contacts { get; set; } = new ObservableCollection<Contact>();

    /// <summary>
    /// Взаимодейстие с окном "о программе"
    /// </summary>
    public Interaction<AboutViewModel, Unit> ShowDialog { get; }

    /// <summary>
    /// 
    /// </summary>
    public Interaction<AddContactViewModel, Contact?> AddContactWindow { get; }

    public MainViewModel()
    {
        ShowDialog = new Interaction<AboutViewModel, Unit>();
        AddContactWindow = new Interaction<AddContactViewModel, Contact?>();
        AboutCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var about = new AboutViewModel();
            await ShowDialog.Handle(about);
        });

        AddContactCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var contact = await AddContactWindow.Handle(new AddContactViewModel());
            if (contact != null)
            {
                if(contact.FullName != "" || contact.FullName != null)
                {
                    Contacts.Add(contact);
                }                
            }                
        });
        GenerateRandomContactsCommand = ReactiveCommand.Create(GenerateRandomContacts);
        Project = ProjectSerializer.LoadFromFile();
        Contacts.AddRange(Project.Contacts);
        SelectedContact = Contacts.First();
        this.WhenAnyValue(x => x.SelectedContact)
            .Subscribe(UpdateContactInfo!);
    }

    public void GenerateRandomContacts()
    {
        Project.Contacts.AddRange(RandomContacts.GenerateRandomContactsName());
        Contacts.AddRange(Project.Contacts);
        ProjectSerializer.SaveToFile(Project);
    }

    private async void UpdateContactInfo(Contact contact)
    {
        if (contact != null)
        {
            CurrentContact.FullName = contact.FullName;
            CurrentContact.Email = contact.Email;
            CurrentContact.Phone = contact.Phone;
            CurrentContact.DateOfBirth = contact.DateOfBirth;
            CurrentContact.IdVk = contact.IdVk;
        }
    }

    [Reactive]
    public Contact SelectedContact
    {
        get => _selectedContact;
        set => this.RaiseAndSetIfChanged(ref _selectedContact, value);
    }

    [Reactive]
    public Contact CurrentContact { get => _currentContact; set => _currentContact = value; }
}
