using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Rendering.Composition;
using ContactsApp.Model;
using DynamicData;
using DynamicData.Binding;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ContactsApp.Avalonia.View.ViewModels;

public class MainViewModel : ViewModelBase
{
    private bool _isDark;

    /// <summary>
    /// Хранит выбранный контакт
    /// </summary>
    private Contact _selectedContact;

    /// <summary>
    /// Хранит индекс выбранного контакта
    /// </summary>
    private int _selectedIndexContact;

    /// <summary>
    /// 
    /// </summary>
    private string? _searchContactText;

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

    public ReactiveCommand<Unit, Unit> ExportContactCommand { get; }

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
                    Project.SortContactsByFullName();
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
                Project.SortContactsByFullName();
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
                    Project.SortContactsByFullName();
                    SelectedContact = Contacts.Count == 0 ? SelectedContact = null : Contacts.First();
                    ProjectSerializer.SaveToFile(Project);
                }
            }
        });

        ExportContactCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (SelectedContact is null) return;

            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.StorageProvider is not { } provider)
                throw new NullReferenceException("Missing StorageProvider instance.");

            var file = await provider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                SuggestedFileName = $"{SelectedContact.FullName}.vcf"
            });

            if (file != null)
            {
                var vcard = BuildVCard(SelectedContact);
                await File.WriteAllTextAsync(file.Path.LocalPath, vcard, Encoding.UTF8);
            }
        });

        GenerateRandomContactsCommand = ReactiveCommand.Create(()=>
            {
                GenerateRandomContacts();
                Project.SortContactsByFullName();
            });

        Project = ProjectSerializer.LoadFromFile();
        Project.SortContactsByFullName();

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

        Project.Contacts
            .ToObservableChangeSet()
            .AutoRefresh()
            .Subscribe(_ =>
            {
                UpdateContactsCollection();
            });
        this.WhenAnyValue(x => x.SelectedContact)
            .Subscribe(UpdateContactInfo!);
        this.WhenAnyValue(x => x.SearchContactText)
         .Throttle(TimeSpan.FromMilliseconds(300))
         .DistinctUntilChanged()
         .Select(searchText => FilterContactsAsync(searchText))
         .Switch() // берём только последний результат
         .ObserveOn(RxApp.MainThreadScheduler)
         .Subscribe(filteredContacts =>
         {
             Contacts.Clear();
             foreach (var contact in filteredContacts)
                 Contacts.Add(contact);
         });
        this.WhenAnyValue(x => x.IsDark)
            .Subscribe(isDark =>
            {
                if (Application.Current is App app)
                {
                    app.SetDarkTheme(isDark);
                }
            });
    }

    /// <summary>
    /// Свойство хранящее выбранный контакт
    /// </summary>
    public Contact SelectedContact
    {
        get => _selectedContact;
        set => this.RaiseAndSetIfChanged(ref _selectedContact, value);
    }

    /// <summary>
    /// Свойство хранящее текущий контакт
    /// </summary>
    public Contact CurrentContact 
    { 
        get => _currentContact; 
        set => this.RaiseAndSetIfChanged(ref _currentContact, value);
    }

    /// <summary>
    /// Свойство хранящеее индекс текушего контакта
    /// </summary>
    public int SelectedIndexContact 
    { 
        get => _selectedIndexContact; 
        set => this.RaiseAndSetIfChanged(ref _selectedIndexContact, value);
    }

    public string? SearchContactText
    {
        get => _searchContactText;
        set => this.RaiseAndSetIfChanged(ref _searchContactText, value);
    }

    public bool IsDark
    {
        get => _isDark;
        set => this.RaiseAndSetIfChanged(ref _isDark, value);
    }

    /// <summary>
    /// Метод вызывающий генерацию создания случайных контактов
    /// </summary>
    public void GenerateRandomContacts()
    {
        Project.Contacts.AddRange(RandomContacts.GenerateRandomContactsName());
        ProjectSerializer.SaveToFile(Project);
    }

    private void UpdateContactsCollection()
    {
        Contacts.Clear();
        foreach (var contact in Project.Contacts)
        {
            Contacts.Add(contact);
        }
    }

    private Task<List<Contact>> FilterContactsAsync(string? searchText)
    {
        return Task.Run(() =>
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                if (Project.Contacts != null)
                {
                    return Project.Contacts.ToList();
                }
                else
                {
                    return new List<Contact>();
                }
            }
            var query = searchText.Trim().ToLowerInvariant();
            var tmp = Project.Contacts;
            // фильтруем только локально, Project.Contacts остаётся нетронутым
            var filtered = tmp
                .Where(c =>
                    (!string.IsNullOrEmpty(c.FullName) && c.FullName.ToLowerInvariant().Contains(query)) ||
                    (!string.IsNullOrEmpty(c.Email) && c.Email.ToLowerInvariant().Contains(query)) ||
                    (!string.IsNullOrEmpty(c.Phone) && c.Phone.ToLowerInvariant().Contains(query)) ||
                    (!string.IsNullOrEmpty(c.IdVk) && c.IdVk.ToLowerInvariant().Contains(query)))
                .ToList();

            return filtered;
        });
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
    /// Builds a vCard string representation of a contact.
    /// </summary>
    /// <param name="contact">The contact to build the vCard for.</param>
    /// <returns>A vCard string representation of the contact.</returns>
    private string BuildVCard(Contact contact)
    {
        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCARD");
        sb.AppendLine("VERSION:3.0");
        sb.AppendLine($"FN:{contact.FullName}");
        sb.AppendLine($"TEL:{contact.Phone}");
        sb.AppendLine($"EMAIL:{contact.Email}");
        sb.AppendLine($"URL:https://vk.com/{contact.IdVk}");
        sb.AppendLine("END:VCARD");
        return sb.ToString();
    }
}
