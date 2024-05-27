using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Environment;

namespace ContactsApp.Model
{
    /// <summary>
    /// Класс сериализации и десериализации
    /// </summary>
    public static class ProjectSerializer
    {
        /// <summary>
        /// Настройка, указывающая как записывать поля, принимающие значение null.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        /// <summary>
        /// Поле хранящая путь до каталога
        /// </summary>
        public static string DirectoryName = GetFolderPath(SpecialFolder.ApplicationData)
            + @"\Zorin\ContactsApp";

        /// <summary>
        /// Поле хранящее путь до файла и его название
        /// </summary>
        public static string FileName = DirectoryName + @"\userdata1.json";

        /// <summary>
        /// Метод реализующий сохранение данных в json файл
        /// </summary>
        /// <param name="project"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void SaveToFile(Project project)
        {
            if (!Directory.Exists(DirectoryName))
            {
                Directory.CreateDirectory(DirectoryName);
                if (!File.Exists(FileName))
                {
                    FileStream fileWithDataContacts = new FileStream(FileName, FileMode.Create);
                    fileWithDataContacts.Close();
                }
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(FileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, project);
            }
        }

        /// <summary>
        /// Метод реализущий чтение данных с json файла
        /// </summary>
        /// <returns>
        /// Возвращает поле класса <see cref="Project">
        /// </returns>
        public static Project LoadFromFile()
        {
            Project project = new Project();
            if (!File.Exists(FileName))
            {
                return project;
            }
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader reader = File.OpenText(FileName))
                {
                    if (reader != null)
                    {
                        project = (Project)serializer.Deserialize(reader,typeof(Project));
                    }
                    else 
                    { 
                        return project; 
                    }
                }
                return project;
            }
            catch (SerializationException)
            {
                return project;
            }
        }
    }
}
