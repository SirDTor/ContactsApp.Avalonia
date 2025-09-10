using ContactsApp.Model;
using DynamicData;
using ReactiveUI.SourceGenerators;
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
using Newtonsoft.Json.Linq;
using DynamicData.Binding;

namespace ContactsApp.Avalonia.View.ViewModels;

public class MainViewModel : ViewModelBase
{
    /// <summary>
    /// Хранит выбранный контакт
    /// </summary>
    private Contact _selectedContact;

    /// <summary>
    /// Хранит индекс выбранного контакта
    /// </summary>
    private int _selectedIndexContact;

    /// <summary>
    /// Хранит текущий контакт
    /// </summary>
    private Contact _currentContact = new Contact();

    /// <summary>
    /// Команда генерации случайных контактов
    /// </summary>
    public ReactiveCommand<Unit, Unit> GenerateRandomContactsCommand { get; }

    /// <summary>
    /// Команда создания нового контакта
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddContactCommand { get; }

    /// <summary>
    /// Команда редактирование контакта
    /// </summary>
    public ReactiveCommand<Unit, Unit> EditContactCommand { get; }

    /// <summary>
    /// Команда удаления контакта
    /// </summary>
    public ReactiveCommand<Unit, Unit> DeleteContactCommand { get; }

    /// <summary>
    /// Команда открытия окна "О программе"
    /// </summary>
    public ReactiveCommand<Unit, Unit> AboutCommand { get; }

    /// <summary>
    /// Класс проекта
    /// </summary>
    public Project Project { get; set; } = new Project();

    /// <summary>
    /// Коллекция контактов
    /// </summary>
    public ObservableCollection<Contact> Contacts { get; set; } = new ObservableCollection<Contact>();

    /// <summary>
    /// Взаимодейстие с окном "о программе"
    /// </summary>
    public Interaction<AboutViewModel, Unit> ShowDialog { get; }

    /// <summary>
    /// Взаимодейстие с окном "Добавить контакт"
    /// </summary>
    public Interaction<AddContactViewModel, Contact?> AddContactWindow { get; }

    /// <summary>
    /// Взаимодейстие с окном "Редактировать контакт"
    /// </summary>
    public Interaction<EditContactViewModel, Contact?> EditContactWindow { get; }

    /// <summary>
    /// Взаимодейстие с окном "подтверждение удаления контакта"
    /// </summary>
    public Interaction<ConfirmationViewModel, bool> ConfirmationWindow { get; }

    public MainViewModel()
    {
        ShowDialog = new Interaction<AboutViewModel, Unit>();
        AddContactWindow = new Interaction<AddContactViewModel, Contact?>();
        EditContactWindow = new Interaction<EditContactViewModel, Contact?>();
        ConfirmationWindow = new Interaction<ConfirmationViewModel, bool>();

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
                if (contact.FullName != "" || contact.FullName != null)
                {
                    Project.Contacts.Add(contact);
                    ProjectSerializer.SaveToFile(Project);
                }
            }
        });

        EditContactCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var index = SelectedIndexContact;
            var tempContact = await EditContactWindow.Handle(new EditContactViewModel(CurrentContact));
            if (tempContact != null)
            {
                Project.Contacts[index] = tempContact;
                SelectedContact = tempContact;
                ProjectSerializer.SaveToFile(Project);
            }
        });

        DeleteContactCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var index = SelectedIndexContact;
            if (index != -1)
            {
                var canDelete = await ConfirmationWindow.Handle(new ConfirmationViewModel(CurrentContact));
                if (canDelete)
                {
                    Project.Contacts.RemoveAt(index);
                    SelectedContact = Contacts.Count == 0 ? SelectedContact = null : Contacts.First();
                    ProjectSerializer.SaveToFile(Project);
                }
            }
        });

        GenerateRandomContactsCommand = ReactiveCommand.Create(GenerateRandomContacts);
        Project = ProjectSerializer.LoadFromFile();

        foreach (var contact in Project.Contacts)
        {
            if (contact.ContactImageByte != null
                && contact.ContactImageByte.Length != 0)
                contact.ContactImage = ImageHelper.ByteToImage(contact.ContactImageByte);
        }

        if (Project.Contacts.Count != 0)
        {
            Contacts.AddRange(Project.Contacts);
            SelectedContact = Contacts.First();
        }

        this.WhenAnyValue(x => x.SelectedContact)
            .Subscribe(UpdateContactInfo!);
        this.WhenAnyValue(x => x.Project.Contacts)
            .Subscribe(x =>
            {
                Project.SortContactsByFullName(Project.Contacts);
                Contacts = Project.Contacts;
            });
    }

    /// <summary>
    /// Метод вызывающий генерацию создания случайных контактов
    /// </summary>
    public void GenerateRandomContacts()
    {
        Project.Contacts.AddRange(RandomContacts.GenerateRandomContactsName());
        ProjectSerializer.SaveToFile(Project);
    }

    /// <summary>
    /// Метож обновления информации о текущем контакте
    /// </summary>
    /// <param name="contact"></param>
    private async void UpdateContactInfo(Contact contact)
    {
        if (contact != null)
        {
            CurrentContact.FullName = contact.FullName;
            CurrentContact.Email = contact.Email;
            CurrentContact.Phone = contact.Phone;
            CurrentContact.DateOfBirth = contact.DateOfBirth;
            CurrentContact.IdVk = contact.IdVk;
            CurrentContact.ContactImage = contact.ContactImage;
        }
        else
        {
            CurrentContact.FullName = "";
            CurrentContact.Email = "test@mail.ru";
            CurrentContact.Phone = "+7(999)999-99-99";
            CurrentContact.DateOfBirth = DateOnly.FromDateTime(new DateTime(2000, 1, 1));
            CurrentContact.IdVk = "";
            CurrentContact.ContactImage = null;
        }
    }

    /// <summary>
    /// Свойство хранящее выбранный контакт
    /// </summary>
    [Reactive]
    public Contact SelectedContact
    {
        get => _selectedContact;
        set => this.RaiseAndSetIfChanged(ref _selectedContact, value);
    }

    /// <summary>
    /// Свойство хранящее текущий контакт
    /// </summary>
    [Reactive]
    public Contact CurrentContact { get => _currentContact; set => _currentContact = value; }

    /// <summary>
    /// Свойство хранящеее индекс текушего контакта
    /// </summary>
    [Reactive]
    public int SelectedIndexContact { get => _selectedIndexContact; set => _selectedIndexContact = value; }
}
