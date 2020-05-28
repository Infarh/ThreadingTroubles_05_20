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
            var result = DoWork(100, 100);

            ResultViewer.Text = result;
        }

        private string DoWork(int IterationCount, int Timeout)
        {
            for (var i = 0; i < IterationCount; i++)
            {
                Debug.WriteLine($"Итерация {i}");
                Thread.Sleep(Timeout);
            }

            return "Result " + DateTime.Now.ToString("hh:mm:ss");
        }
    }
}
