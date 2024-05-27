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
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace ContactsApp.Avalonia.View.ViewModels
{
    public class AddContactViewModel : ViewModelBase
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
        /// Команда ОК
        /// </summary>
        public ReactiveCommand<Unit, Contact> AddContactCommand { get; }

        public Contact Contact { get; set; } = new Contact();

        public AddContactViewModel()
        {
            AddContactCommand = ReactiveCommand.Create(() =>
            {
                Contact.FullName = FullName;
                Contact.Email = Email;
                Contact.Phone = Phone;
                Contact.DateOfBirth = new DateOnly(DateOfBirth.Year, DateOfBirth.Month, DateOfBirth.Day);
                Contact.IdVk = IdVk;
                return Contact;
            });
        }

        [Reactive]
        [MaxLength(100)]
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
                if (value.Length >= 100)
                {
                    throw new ArgumentException($"Name:\n->Contact name must be less than 100, " +
                        $"value = {value.Length}\n");
                }
                else
                {
                    TextInfo toUpperTextInfo = CultureInfo.CurrentCulture.TextInfo;
                    //_fullName = toUpperTextInfo.ToTitleCase(value).ToString();
                    this.RaiseAndSetIfChanged(ref _fullName, toUpperTextInfo.ToTitleCase(value).ToString());
                }
            }
        }

        [Reactive]
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
                if (value.Length >= 100)
                {
                    throw new ArgumentException($"Email:\n->Contact email must be less than 100," +
                        $" value = {value.Length}\n");
                }
                this.RaiseAndSetIfChanged(ref _email, value);
            }
        }

        [Reactive]
        [Phone]
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

        [Reactive]
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

        [Reactive]
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
                if (value.Length >= 50)
                {
                    throw new ArgumentException($"IdVK:\n->Contact ID must be less than 50, " +
                        $"value = {value.Length}\n");
                }
                this.RaiseAndSetIfChanged(ref _idVk, value);
            }
        }
    }
}
