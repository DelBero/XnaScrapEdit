using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CogaenEditor2.GUI.Windows
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        public ConnectionWindow()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            App app = (App)App.Current;
            app.MessageHandler.connect(textBoxIp.Text, "80");
            Hide();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

    }
}
