using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Threading;

namespace ThreadingTroubles
{
    class Program
    {
        private const string data_dir = "data";

        static void Main(string[] args)
        {
            var data_directory = new DirectoryInfo(data_dir);

            var words_count = 0;
          

            var files = data_directory.GetFiles();

            var timer = new Stopwatch();
            



            Console.ReadLine();
        }

        private static readonly char[] __Separators = { ' ', '.', ',', '!', ';', '-', '(', ')', '[', ']', '{', '}' };

        private static readonly object __LockObject = new object();
        private static void PrintConsoleData(string Message, int Count, int Timeout = 10)
        {
            var thread_id = Thread.CurrentThread.ManagedThreadId;
            lock (__LockObject)
                Console.WriteLine("Поток {0} запущен", thread_id);

            for (var i = 0; i < Count; i++)
            {
                lock (__LockObject)
                {
                    Console.Write("{0,4}:", i);
                    Console.Write(Message);
                    Console.WriteLine(" - thread id:{0}", thread_id);
                }

                Thread.Sleep(Timeout);
            }

            lock (__LockObject)
                Console.WriteLine("Поток {0} завершён", thread_id);
        }

        private static int GetWordsCount(FileInfo file)
        {
            Console.WriteLine("Обработка файла {0} - thread id: {1}", file.Name, Thread.CurrentThread.ManagedThreadId);

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
