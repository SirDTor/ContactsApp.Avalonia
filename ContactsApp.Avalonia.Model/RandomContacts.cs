using System;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using System.IO;

namespace ContactsApp.Model
{
    /// <summary>
    /// Класс генерации случайных контактов с автоматическими аватарами
    /// </summary>
    public static class RandomContacts
    {
        private static Project project = new Project();
        private static Random randomNumber = new Random();

        private static string[] surnames = { "Смирнов", "Долгов", "Беспалов",
            "Малышев", "Толкачев", "Лазарев", "Яковлев", "Демин", "Журавлев", "Кондрашов" };
        private static string[] maleNames = { "Тимур", "Михаил", "Иван", "Дмитрий",
            "Николай", "Тимофей" };
        private static string[] femaleNames = { "Амбразура", "София", "Анна",
            "Полина", "Елизавета", "Александра", "Анна" };
        private static string[] patronymics = { "Амбразуров", "Матвеев", "Михайлов", "Егоров",
            "Алексеев", "Григорьев", "Александров", "Глебов", "Даниилов" };

        private static string[] fullNames = new string[20];
        private static string[] emails = { "o@outlook.com", "hr6zdl@yandex.ru", "kaft93x@outlook.com",
            "dcu@yandex.ru", "19dn@outlook.com", "pa5h@mail.ru", "281av0@gmail.com", "8edmfh@outlook.com",
            "sfn13i@mail.ru", "g0orc3x1@outlook.com", "rv7bp@gmail.com", "93@outlook.com", "er@gmail.com",
            "o0my@gmail.com", "715qy08@gmail.com", "vubx0t@mail.ru", "wnhborq@outlook.com", "gq@yandex.ru",
            "ic0pu@outlook.com", "o7khr@yandex.ru" };
        private static string[] phones = { "8(923)480-65-83", "8(923)234-84-20", "8(923)136-38-75",
            "8(923)653-03-02", "8(923)824-41-77", "8(923)112-54-20", "8(923)678-54-33",
            "8(923)971-37-41", "8(923)760-83-72", "8(923)950-38-18" };
        private static string[] idVk = { "id8626810", "id2307938", "id7027388", "id6938923",
            "id2570718", "id7381412", "id1153817", "id7013733", "id8111215", "id6492395" };

        private static int GenerateDigit(int lengthOfArray) => randomNumber.Next(lengthOfArray);

        /// <summary>
        /// Генерация случайных контактов с аватарками
        /// </summary>
        public static ObservableCollection<Contact> GenerateRandomContactsName()
        {
            project.Contacts.Clear();

            for (int i = 0; i <= 10; i++)
            {
                fullNames[i] = surnames[GenerateDigit(surnames.Length)] + " " +
                    maleNames[GenerateDigit(maleNames.Length)] + " " +
                    patronymics[GenerateDigit(patronymics.Length)] + "ич";
            }

            for (int i = 11; i < 20; i++)
            {
                fullNames[i] = surnames[GenerateDigit(surnames.Length)] + "а " +
                    femaleNames[GenerateDigit(femaleNames.Length)] + " " +
                    patronymics[GenerateDigit(patronymics.Length)] + "на";
            }

            for (int i = 0; i < 20; i++)
            {
                var contact = new Contact(
                    fullNames[GenerateDigit(fullNames.Length)],
                    emails[GenerateDigit(emails.Length)],
                    phones[GenerateDigit(phones.Length)],
                    DateOnly.FromDateTime(DateTime.Today),
                    idVk[GenerateDigit(idVk.Length)]
                );

                // Генерируем случайный аватар
                var avatarResult = AvatarGenerator.GenerateRandomAvatar(64, 64);
                if (avatarResult.HasValue)
                {
                    contact.ContactImage = avatarResult.Value.bitmap;
                    contact.ContactImageByte = avatarResult.Value.bytes;
                }
                else
                {
                    // На случай, если генерация не удалась, можно поставить заглушку
                    contact.ContactImage = null;
                    contact.ContactImageByte = Array.Empty<byte>();
                }


                project.Contacts.Add(contact);
            }

            return project.Contacts;
        }
    }
}
