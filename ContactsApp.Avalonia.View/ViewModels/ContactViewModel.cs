using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ContactsApp.Model;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;

namespace ContactsApp.Avalonia.View.ViewModels
{
    public class ContactViewModel : ViewModelBase
    {
        #region Constructor and Model

        public ContactViewModel(Contact contact)
        {
            Contact = contact ?? throw new ArgumentNullException(nameof(contact));
        }

        public ContactViewModel() : this(new Contact()) { }

        public Contact Contact { get; }

        #endregion

        #region Properties (Proxies for Model)

        public string FullName
        {
            get => Contact.FullName;
            set
            {
                if (Contact.FullName != value) 
                {
                    Contact.FullName = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string Email
        {
            get => Contact.Email;
            set
            {
                if (Contact.Email != value)
                {
                    Contact.Email = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string Phone
        {
            get => Contact.Phone;
            set
            {
                if (Contact.Phone != value)
                {
                    Contact.Phone = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string IdVk
        {
            get => Contact.IdVk;
            set
            {
                if (Contact.IdVk != value)
                {
                    Contact.IdVk = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Bitmap? ContactImage
        {
            get => Contact.ContactImage;
            set
            {
                if (Contact.ContactImage != value)
                {
                    Contact.ContactImage = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public byte[]? ContactImageByte
        {
            get => Contact.ContactImageByte;
            set
            {
                if (Contact.ContactImageByte != value)
                {
                    Contact.ContactImageByte = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Date Binding Adapter

        // В модели DateOnly, а Avalonia DatePicker требует DateTimeOffset? 
        // Поэтому добавляем адаптер-свойство:
        public DateTimeOffset? DateOfBirthOffset
        {
            get => new DateTimeOffset(Contact.DateOfBirth.ToDateTime(TimeOnly.MinValue));
            set
            {
                if (value.HasValue)
                    Contact.DateOfBirth = DateOnly.FromDateTime(value.Value.Date);
            }
        }

        #endregion

        #region File Picker

        public async Task<IStorageFile?> GetPath()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.StorageProvider is not { } provider)
                throw new NullReferenceException("Missing StorageProvider instance.");

            var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Contact Image",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Image files")
                    {
                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" },
                        AppleUniformTypeIdentifiers = new[] { "public.image" },
                        MimeTypes = new[] { "image/*" }
                    }
                }
            });

            return files.Count > 0 ? files[0] : null;
        }

        #endregion
    }
}
