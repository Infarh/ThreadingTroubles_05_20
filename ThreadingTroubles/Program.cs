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

            var manual_reset_event = new ManualResetEvent(false);
            var auto_reset_event = new AutoResetEvent(false);

            //var thread = new Thread(() =>
            //{
            //    Console.WriteLine("Вторичный поток стартовал!");
            //    Thread.Sleep(500);

            //    Console.WriteLine("Вторичный поток ждёт разрешения на дальнейшую работу...");
            //    manual_reset_event.WaitOne();
            //    Console.WriteLine("\t разрешение получено!");

            //    Thread.Sleep(500);
            //    Console.WriteLine("Вторичный поток завершился!");
            //});
            //thread.Start();

            //Console.WriteLine("Главный поток готов возобновить работу вторичного...");
            //Console.ReadLine();
            //manual_reset_event.Set();

            var threads = new Thread[10];

            for (var i = 0; i < threads.Length; i++)
            {
                var index = i;
                var thread = new Thread(() =>
                {
                    Console.WriteLine("Поток {0} запущен и ждёт разрешения.", index);
                    Thread.Sleep(500);

                    auto_reset_event.WaitOne();

                    Thread.Sleep(500);
                    Console.WriteLine("Поток {0} выполнил свою работу.", index);
                });
                thread.Start();
                threads[i] = thread;
            }

            Console.WriteLine("Главный поток готов разрешить работу...");

            for (var i = 0; i < threads.Length; i++)
            {
                Console.ReadLine();
                auto_reset_event.Set();
            }

            var sync_root = new object();
            lock (sync_root)
            {

            }

            Monitor.Enter(sync_root);
            try
            {
                // Критическая секция
            }
            finally
            {
                Monitor.Exit(sync_root);
            }


            Console.WriteLine("Главный поток завершил свою работу!");
            Console.ReadLine();
        }

        private static void ThreadMethod()
        {
            Console.WriteLine("Вторичный поток стартовал!");

            Thread.Sleep(1000);

            Console.WriteLine("Вторичный поток завершился!");
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
