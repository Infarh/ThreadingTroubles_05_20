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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //DoProcess();
            DoProcessInThread();
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
            for (var i = 0; i < IterationCount; i++)
            {
                Debug.WriteLine($"Итерация {i}");
                ProgressInfo?.Invoke((double)i / IterationCount);
                Thread.Sleep(Timeout);
            }

            ProgressInfo?.Invoke(1);
            return "Result " + DateTime.Now.ToString("hh:mm:ss");
        }
    }
}
