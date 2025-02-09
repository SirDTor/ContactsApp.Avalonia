using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReactiveUI;
using Avalonia.Controls.ApplicationLifetimes;
using System.IO;
using Avalonia;

namespace ContactsApp.Avalonia.View.ViewModels
{
    public class ContactViewModel : ViewModelBase
    {
        /// <summary>
        /// Возвращает или задает полное имя контакта
        /// </summary>
        /// <summary>
        /// Полное имя контакта
        /// </summary>
        private string _fullName;

        /// <summary>
        /// Email контакта
        /// </summary>
        private string _email;

        /// <summary>
        /// Номер телефона контакта
        /// </summary>
        private string _phone;

        /// <summary>
        /// Дата рождения контакта
        /// </summary>
        private DateTimeOffset _dateOfBirth = new(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);

        /// <summary>
        /// ID ВКонтакте контакта
        /// </summary>
        private string _idVk;

        /// <summary>
        /// Регулярное выражение для номера телефона
        /// Пример: +7(000)000-00-00
        /// </summary>
        private const string _phoneNumberValidationMask =
                    @"^((\+7|7|8)[[\(]?(\d{3})[\)]?]?\d{3}[[-]?(\d{2}[-]?]?\d{2}))$";

        /// <summary>
        /// 
        /// </summary>
        private IStorageFile _imagePath;

        /// <summary>
        /// 
        /// </summary>
        private Bitmap _contactImage;

        /// <summary>
        /// 
        /// </summary>
        private byte[] _contactImageByte = [];


        [MaxLength(100)]
        [Required]
        /// <summary>
        /// Возвращает или задает полное имя контакта
        /// </summary>
        public string FullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                TextInfo toUpperTextInfo = CultureInfo.CurrentCulture.TextInfo;
                this.RaiseAndSetIfChanged(ref _fullName, toUpperTextInfo.ToTitleCase(value).ToString());
            }
        }

        [EmailAddress]
        /// <summary>
        /// Возвращает или задает email контакта
        /// </summary>
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _email, value);
            }
        }

        [Phone]
        [Required]
        /// <summary>
        /// Возвращает или задает номер телефона контакта
        /// </summary>
        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                if (!Regex.IsMatch(value, _phoneNumberValidationMask))
                {
                    throw new ArgumentException($"PhoneNumber:\n->The phone number contains an" +
                        $" invalid character.\nExample:\n" +
                        $"8(923)442-79-25\n" +
                        $"89234427925\n");
                }
                this.RaiseAndSetIfChanged(ref _phone, value);
            }
        }

        [Required]
        /// <summary>
        /// Возвращает или задает дату рождения контакта
        /// </summary>
        public DateTimeOffset DateOfBirth
        {
            get
            {
                return _dateOfBirth;
            }
            set
            {
                if (value.Year <= 1900 || value > DateTimeOffset.Now)
                {
                    throw new ArgumentException($"Date:\n->Year must be less or more than " +
                        $"current year But was {value.Year}\n");
                }
                this.RaiseAndSetIfChanged(ref _dateOfBirth, value);

            }
        }

        [MaxLength(50)]
        /// <summary>
        /// Возвращает или задает ID ВКонтакте контакта
        /// </summary>
        public string IdVk
        {
            get
            {
                return _idVk;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _idVk, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Reactive]
        public byte[] ContactImageByte { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Bitmap ContactImage
        {
            get => _contactImage;
            set => this.RaiseAndSetIfChanged(ref _contactImage, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<IStorageFile> GetPath()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
                throw new NullReferenceException("Missing StorageProvider instance.");
            // Start async operation to open the dialog.
            var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Contact Image",
                AllowMultiple = false,
                FileTypeFilter = new FilePickerFileType[]
                {
                    new("Image")
                    {
                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp" },
                        AppleUniformTypeIdentifiers = new[] { "public.image" } ,
                        MimeTypes = new[] { "image/*" }
                    }
                }
            });
            if (files.Count >= 1)
            {
                // Open reading stream from the first file.
                await using var stream = await files[0].OpenReadAsync();
                using var streamReader = new StreamReader(stream);
                // Reads all the content of file as a text.
                var fileContent = await streamReader.ReadToEndAsync();
            }
            return files?.Count >= 1 ? files[0] : null;
        }
    }
}
