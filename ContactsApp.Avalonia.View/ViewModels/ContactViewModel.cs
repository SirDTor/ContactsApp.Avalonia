using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using System.IO;
using Avalonia;

namespace ContactsApp.Avalonia.View.ViewModels
{
    public partial class ContactViewModel : ViewModelBase
    {
        #region Constants

        private const string PhoneNumberValidationMask =
            @"^(\+7|7|8)\s*\(?\d{3}\)?\s*\d{3}[- ]?\d{2}[- ]?\d{2}$";

        #endregion

        #region Private fields

        private string _fullName = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private DateTimeOffset _dateOfBirth = new(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
        private string _idVk = string.Empty;
        private Bitmap? _contactImage;

        [Reactive]
        private byte[] _contactImageByte = Array.Empty<byte>();

        #endregion

        #region Properties with validation

        [MaxLength(100)]
        [Required]
        public string FullName
        {
            get => _fullName;
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(FullName));
                var textInfo = CultureInfo.CurrentCulture.TextInfo;
                this.RaiseAndSetIfChanged(ref _fullName, textInfo.ToTitleCase(value));
            }
        }

        [EmailAddress]
        public string Email
        {
            get => _email;
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(Email));
                this.RaiseAndSetIfChanged(ref _email, value);
            }
        }

        [Phone]
        [Required]
        public string Phone
        {
            get => _phone;
            set
            {
                if (!Regex.IsMatch(value, PhoneNumberValidationMask))
                {
                    throw new ArgumentException(
                        "PhoneNumber:\n->Invalid format.\nExamples:\n" +
                        "8(923)442-79-25\n89234427925\n+7(923)442-79-25\n");
                }
                this.RaiseAndSetIfChanged(ref _phone, value);
            }
        }

        [Required]
        public DateTimeOffset DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                if (value.Year <= 1900 || value > DateTimeOffset.Now)
                {
                    throw new ArgumentException(
                        $"Date:\n->Year must be > 1900 and not in the future. But was {value:yyyy-MM-dd}\n");
                }
                this.RaiseAndSetIfChanged(ref _dateOfBirth, value);
            }
        }

        [MaxLength(50)]
        public string IdVk
        {
            get => _idVk;
            set => this.RaiseAndSetIfChanged(ref _idVk, value);
        }

        public Bitmap? ContactImage
        {
            get => _contactImage;
            set => this.RaiseAndSetIfChanged(ref _contactImage, value);
        }

        #endregion

        #region Methods

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
                    new FilePickerFileType("Image")
                    {
                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp" },
                        AppleUniformTypeIdentifiers = new[] { "public.image" },
                        MimeTypes = new[] { "image/*" }
                    }
                }
            });

            return files?.Count >= 1 ? files[0] : null;
        }

        #endregion
    }
}
