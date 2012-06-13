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
using CogaenEditorConnect.Communication;
using CogaenEditExtension.Communication;

namespace CogaenEditExtension
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        private MessageHandler m_msgHandler = null;

        public ConnectionWindow(MessageHandler msgHandler)
        {
            m_msgHandler = msgHandler;
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            m_msgHandler.connect(textBoxIp.Text, "80");
            Hide();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

    }
}
