using ContactsApp.Model;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
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
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;
using Avalonia;

namespace ContactsApp.Avalonia.View.ViewModels
{
    public class AddContactViewModel : ContactViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public AddContactViewModel()
        {
            var canAddContact = this.WhenAnyValue(x => x.FullName, x => x.Phone,
                (fullname, phone) =>
                !string.IsNullOrWhiteSpace(fullname) &&
                !string.IsNullOrWhiteSpace(phone))
                .DistinctUntilChanged();
            
            AddContactCommand = ReactiveCommand.Create(() =>
            {
                Contact.FullName = FullName;
                Contact.Email = Email;
                Contact.Phone = Phone;
                Contact.DateOfBirth = new DateOnly(DateOfBirthOffset.Value.Year, DateOfBirthOffset.Value.Month, DateOfBirthOffset.Value.Day);
                Contact.IdVk = IdVk;
                Contact.ContactImage = ContactImage;
                Contact.ContactImageByte = ContactImageByte;
                return Contact;
            }, canAddContact);

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
        public ReactiveCommand<Unit, Contact> AddContactCommand { get; }

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
