using ReactiveUI;
using ReactiveUI.SourceGenerators; // важно: генераторы, а не Fody
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Avalonia.Media.Imaging;

namespace ContactsApp.Model
{
    /// <summary>Описывает контакт</summary>
    public partial class Contact : ReactiveObject, ICloneable
    {
        #region Constants

        /// <summary>
        /// Регулярка для телефона (поддерживает: +7(000)000-00-00, 8(000)0000000, 89234427925 и т.п.)
        /// </summary>
        private const string PhoneRegex =
            @"^(\+7|7|8)\s*\(?\d{3}\)?\s*\d{3}[- ]?\d{2}[- ]?\d{2}$";

        #endregion

        #region Private fields

        private string _fullName = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private DateOnly _dateOfBirth;
        private string _idVk = string.Empty;

        #endregion

        #region Validated properties

        [DataMember]
        public string FullName
        {
            get => _fullName;
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(FullName));
                if (value.Length >= 100)
                    throw new ArgumentException($"Name:\n->Contact name must be less than 100, value = {value.Length}\n");

                var ti = CultureInfo.CurrentCulture.TextInfo;
                var normalized = ti.ToTitleCase(value);
                this.RaiseAndSetIfChanged(ref _fullName, normalized);
            }
        }

        [DataMember]
        public string Email
        {
            get => _email;
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(Email));
                if (value.Length >= 100)
                    throw new ArgumentException($"Email:\n->Contact email must be less than 100, value = {value.Length}\n");
                if (!new EmailAddressAttribute().IsValid(value))
                    throw new ArgumentException("Email:\n->Invalid email format.\n");

                this.RaiseAndSetIfChanged(ref _email, value);
            }
        }

        [DataMember]
        public string Phone
        {
            get => _phone;
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(Phone));
                if (!Regex.IsMatch(value, PhoneRegex))
                    throw new ArgumentException(
                        "PhoneNumber:\n->Invalid format.\nExamples:\n8(923)442-79-25\n89234427925\n+7(923)442-79-25\n");

                this.RaiseAndSetIfChanged(ref _phone, value);
            }
        }

        [DataMember]
        public DateOnly DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                if (value.Year <= 1900 || value > today)
                    throw new ArgumentException($"Date:\n->Year must be > 1900 and <= current date. But was {value:yyyy-MM-dd}\n");

                this.RaiseAndSetIfChanged(ref _dateOfBirth, value);
            }
        }

        [DataMember]
        public string IdVk
        {
            get => _idVk;
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(IdVk));
                if (value.Length >= 50)
                    throw new ArgumentException($"IdVK:\n->Contact ID must be less than 50, value = {value.Length}\n");

                this.RaiseAndSetIfChanged(ref _idVk, value);
            }
        }

        #endregion

        #region Auto-generated properties

        [Reactive]
        private Bitmap _contactImage;

        [property: DataMember] // прокинем атрибут на сгенерённое свойство
        [Reactive]
        private byte[] _contactImageByte = Array.Empty<byte>();

        #endregion

        #region Constructors

        public Contact(string fullName, string email, string phone, DateOnly dateOfBirth, string idVk)
        {
            FullName = fullName;
            Email = email;
            Phone = phone;
            DateOfBirth = dateOfBirth;
            IdVk = idVk;
        }

        public Contact() { }

        #endregion

        #region Methods

        public object Clone() => MemberwiseClone();

        #endregion
    }
}
