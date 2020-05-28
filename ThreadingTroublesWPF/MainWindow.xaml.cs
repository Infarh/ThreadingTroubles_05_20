using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ThreadingTroublesWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            button.IsEnabled = false;

            //DoProcess();
            //DoProcessInThread();

            //var result = await DoWorkAsync(100, 100);
            var progress = new Progress<double>(p => ProgressBar.Value = p);

            var result = await Task.Run(() => DoWorkAsync(100, 100, progress));

            ResultViewer.Text = result;
            button.IsEnabled = true;
        }

        private static async Task<string> DoWorkAsync(int IterationCount, int Timeout, IProgress<double> Progress = null)
        {
            var thread_id = Thread.CurrentThread.ManagedThreadId;
            for (var i = 0; i < IterationCount; i++)
            {
                Debug.WriteLine($"Итерация {i} - поток {thread_id}");
                Progress?.Report((double)i / IterationCount);
                await Task.Delay(Timeout);
            }

            return "Result " + DateTime.Now.ToString("hh:mm:ss");
        }

        private void DoProcessInThread()
        {
            var thread = new Thread(DoProcess)
            {
                IsBackground = true
            };
            thread.Start();
        }

        private void ProgressInformator(double Progress)
        {
            if (Dispatcher.CheckAccess())
                ProgressBar.Value = Progress;
            else
                Dispatcher.Invoke(() => { ProgressBar.Value = Progress; });
        }

        private void DoProcess()
        {
            var result = DoWork(100, 100, ProgressInformator);
            //ResultViewer.Text = result;
            //Application.Current.Dispatcher
            if (ResultViewer.Dispatcher.CheckAccess())
            {
                ResultViewer.Text = result;
            }
            else
                ResultViewer.Dispatcher.Invoke(() =>
                {
                    ResultViewer.Text = result;
                });
        }

        private static string DoWork(int IterationCount, int Timeout, Action<double> ProgressInfo = null)
        {
            var thread_id = Thread.CurrentThread.ManagedThreadId;
            for (var i = 0; i < IterationCount; i++)
            {
                Debug.WriteLine($"Итерация {i} - поток {thread_id}");
                ProgressInfo?.Invoke((double)i / IterationCount);
                Thread.Sleep(Timeout);
            }

            ProgressInfo?.Invoke(1);
            return "Result " + DateTime.Now.ToString("hh:mm:ss");
        }
    }
}
