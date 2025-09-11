using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ContactsApp.Model
{
    /// <summary>
    /// Описывает список контактов
    /// </summary>
    public class Project : ReactiveObject
    {
        private bool _isSorting = false;

        [DataMember]
        /// <summary>
        /// Возвращает или задает список контактов
        /// </summary>
        public ObservableCollection<Contact> Contacts { get; set; } = new ObservableCollection<Contact>();

        /// <summary>
        /// Возвращает отсортированный список контактов
        /// </summary>
        public void SortContactsByFullName()
        {
            if (_isSorting || Contacts == null || Contacts.Count == 0) return;

            _isSorting = true;
            try
            {
                var sortedContacts = Contacts.OrderBy(c => c.FullName).ToList();
                Contacts.Clear();
                foreach (var contact in sortedContacts)
                {
                    Contacts.Add(contact);
                }
            }
            finally
            {
                _isSorting = false;
            }
        }

        /// <summary>
        /// Возвращает список именинников
        /// </summary>
        /// <param name="contacts"></param>
        /// <returns></returns>
        public ObservableCollection<Contact> FindBirthdayContacts(ObservableCollection<Contact> contacts)
        {
            var birthdayContacts = (ObservableCollection<Contact>)contacts.Where(contact => contact.DateOfBirth.Day == DateTime.Today.Day
            && contact.DateOfBirth.Month == DateTime.Today.Month);
            return birthdayContacts;
        }

        /// <summary>
        /// Возвращает найденные по подстроке контакты
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ObservableCollection<Contact> FindContacts(ObservableCollection<Contact> contacts, string contactName)
        {
            if (contactName != "")
            {
                var selectedContact = (ObservableCollection<Contact>)contacts.Where(contact => contact.FullName.Contains(contactName));
                return selectedContact;
            }
            else return contacts;
        }
    }
}

