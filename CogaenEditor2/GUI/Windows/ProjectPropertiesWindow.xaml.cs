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
using System.Windows.Forms;
using CogaenEditor2.Manager;

namespace CogaenEditor2.GUI.Windows
{
    /// <summary>
    /// Interaction logic for ProjectPropertiesWindow.xaml
    /// </summary>
    public partial class ProjectPropertiesWindow : Window
    {
        private static ProjectPropertiesWindow m_instance = null;

        private ProjectPropertiesWindow()
        {
            InitializeComponent();
        }

        public static ProjectPropertiesWindow GetInstance()
        {
            if (m_instance == null)
            {
                m_instance = new ProjectPropertiesWindow();
            }
            return m_instance;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_instance = null;
            this.Hide();
            //e.Cancel = true;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (DialogResult != null)
                DialogResult = true;
            this.Close();
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            DialogResult result = fb.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Project data = DataContext as Project;
                if (data != null)
                {
                    data.ExportDirectory = fb.SelectedPath;
                }
            }
        }
    }
}
