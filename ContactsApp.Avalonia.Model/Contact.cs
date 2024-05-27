using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace ContactsApp.Model
{
    /// <summary>
    /// Описывает контакт
    /// </summary>
    public class Contact: ReactiveObject, ICloneable 
    {
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
        private DateOnly _dateOfBirth;

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

        [DataMember]
        [Reactive]
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

        [DataMember]
        [Reactive]
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
                _email = value;
            }
        }

        [DataMember]
        [Reactive]
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
                _phone = value;
            }
        }

        [DataMember]
        [Reactive]
        /// <summary>
        /// Возвращает или задает дату рождения контакта
        /// </summary>
        public DateOnly DateOfBirth
        {
            get
            {
                return _dateOfBirth;
            }
            set
            {
                if (value.Year <= 1900 || value > DateOnly.FromDateTime(DateTime.Now))
                {
                    throw new ArgumentException($"Date:\n->Year must be less or more than " +
                        $"current year But was {value.Year}\n");
                }
                _dateOfBirth = value;
                
            }
        }

        [DataMember]
        [Reactive]
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
                _idVk = value;
            }
        }

        /// <summary>
        /// Создает экземпляр <see cref="Contact">
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="email"></param>
        /// <param name="phone"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="idVk"></param>
        public Contact(string fullName, string email, string phone, 
            DateOnly dateOfBirth, string idVk)
        {
            FullName = fullName;
            Email = email;
            Phone = phone;
            DateOfBirth = dateOfBirth;
            IdVk = idVk;
        }

        /// <summary>
        /// Создает пустой экземпляр <see cref="Contact"/>
        /// </summary>
        public Contact() { }

        /// <summary>
        /// Клонирует экзмепляр класса
        /// </summary>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
