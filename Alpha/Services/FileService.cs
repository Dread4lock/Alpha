using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alpha.Services
{
    public class FileService
    {
        public string OpenFile()
        {
            // Метод для открытия диалогового окна для выбора файла
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                Title = "Select Alphabet File"
            };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }

        public List<char> LoadLetters(string filePath)
        {
            // Метод для чтения букв из файла и фильтрации символов
            return File.ReadAllText(filePath).Where(char.IsLetter).ToList();
        }
    }
}
