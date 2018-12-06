using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
