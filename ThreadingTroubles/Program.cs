using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace ThreadingTroubles
{
    class Program
    {
        private const string data_dir = "data";

        static void Main(string[] args)
        {
            var data_directory = new DirectoryInfo(data_dir);

            var words_count = 0;
            foreach (var file in data_directory.GetFiles())
            {
                Console.WriteLine("{0} - {1}kb", file.Name, file.Length / 1024.0);

                words_count += GetWordsCount(file);
            }

            Console.WriteLine("Число слов {0}", words_count);

            Console.ReadLine();
        }

        private static char[] __Separators = { ' ', '.', ',', '!', ';', '-', '(', ')', '[', ']', '{', '}' };

        private static int GetWordsCount(FileInfo file)
        {
            if (!file.Exists) throw new FileNotFoundException("Файл для анализа не найден", file.FullName);
            if (file.Length == 0) return 0;

            var count = 0;
            using (var reader = file.OpenText())
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var words = line.Split(__Separators, StringSplitOptions.RemoveEmptyEntries);

                    count += words.Length;
                }

            return count;
        }
    }
}
