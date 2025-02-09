using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ContactsApp.Model;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.Avalonia.View.ViewModels
{
    public class EditContactViewModel : ContactViewModel
    {
        public EditContactViewModel(Contact contact)
        {
            FullName = contact.FullName;
            Email = contact.Email;
            Phone = contact.Phone;
            DateOfBirth = new DateTimeOffset(contact.DateOfBirth, new TimeOnly(0), new TimeSpan(0));
            IdVk = contact.IdVk;
            ContactImage = contact.ContactImage;


            var canEditContact = this.WhenAnyValue(x => x.FullName, x => x.Phone,
               (fullname, phone) =>
               !string.IsNullOrWhiteSpace(fullname) &&
               !string.IsNullOrWhiteSpace(phone))
               .DistinctUntilChanged();

            EditContactCommand = ReactiveCommand.Create(() =>
            {
                Contact.FullName = FullName;
                Contact.Email = Email;
                Contact.Phone = Phone;
                Contact.DateOfBirth = new DateOnly(DateOfBirth.Year, DateOfBirth.Month, DateOfBirth.Day);
                Contact.IdVk = IdVk;
                Contact.ContactImage = ContactImage;
                Contact.ContactImageByte = ContactImageByte;
                return Contact;
            }, canEditContact);

            OpenContactImageCommand = ReactiveCommand.Create(async () =>
            {
                ImagePath = await GetPath();
                if (ImagePath is null) return;
                var bitmap = new Bitmap(ImagePath.Path.AbsolutePath);
                await Task.Run(() =>
                {
                    using (MemoryStream ms = new())
                    {
                        bitmap.Save(ms);
                        ContactImageByte = ms.ToArray();
                    }
                });
                ContactImage = bitmap;
            });
        }

        /// <summary>
        /// Команда ОК
        /// </summary>
        public ReactiveCommand<Unit, Contact> EditContactCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        public ReactiveCommand<Unit, Task> OpenContactImageCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        public Contact Contact { get; set; } = new Contact();

        /// <summary>
        /// 
        /// </summary>
        [Reactive]
        public IStorageFile ImagePath { get; set; }

    }
}
