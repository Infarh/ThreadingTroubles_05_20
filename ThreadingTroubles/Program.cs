using System;
using System.Collections.Generic;
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
            //var data_directory = new DirectoryInfo(data_dir);

            //var words_count = 0;
            //foreach (var file in data_directory.GetFiles()) // 468 152
            //{
            //    Console.WriteLine("{0} - {1}kb", file.Name, file.Length / 1024.0);

            //    words_count += GetWordsCount(file);
            //}

            //Console.WriteLine("Число слов {0} - последовательно", words_count);


            //var files = data_directory.GetFiles();

            //var threads = new Thread[files.Length];
            //words_count = 0;
            //for (var i = 0; i < threads.Length; i++) //127 912
            //{
            //    var file_to_process = files[i];
            //    threads[i] = new Thread(() => words_count = words_count + GetWordsCount(file_to_process));
            //    threads[i].Start();
            //}

            //for (var i = 0; i < threads.Length; i++)
            //    threads[i].Join();

            //Console.WriteLine("Число слов {0} параллельно", words_count);

            var print_threads = new Thread[10];
            for (var i = 0; i < print_threads.Length; i++)
            {
                var message = $"Thread {i + 1} message";
                var thread = new Thread(() => PrintConsoleData(message, 10, 0));
                thread.Start();
                print_threads[i] = thread;
            }

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
