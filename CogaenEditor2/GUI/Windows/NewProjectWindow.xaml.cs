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
using System.ComponentModel;
using System.Windows.Forms;
using CogaenDataItems.Exporter;

namespace CogaenEditor2.GUI.Windows
{
    /// <summary>
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        private static NewProjectWindow m_instance = null;

        private NewProjectWindow()
        {
            InitializeComponent();
        }

        public static NewProjectWindow GetInstance()
        {
            if (m_instance == null)
            {
                m_instance = new NewProjectWindow();
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
            DialogResult = true;
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            DialogResult result = fb.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                NewProjectData data = DataContext as NewProjectData;
                data.Directory = fb.SelectedPath;
            }
        }

        private void FileExport_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            DialogResult result = fb.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                NewProjectData data = DataContext as NewProjectData;
                data.ExportDirectory = fb.SelectedPath;
            }
        }

        private void FileConfig_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fb = new SaveFileDialog();
            DialogResult result = fb.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                NewProjectData data = DataContext as NewProjectData;
                data.ExportDirectory = fb.FileName;
            }
        }
        
    }
    public class NewProjectData : INotifyPropertyChanged
    {
        private String m_name;

        public String Name
        {
            get { return m_name; }
            set { m_name = value; OnPropertyChanged("Name"); }
        }

        private String m_Directory;

        public String Directory
        {
            get { return m_Directory; }
            set { m_Directory = value; OnPropertyChanged("Directory"); }
        }

        private String m_exportDirectory;

        public String ExportDirectory
        {
            get { return m_exportDirectory; }
            set { m_exportDirectory = value; OnPropertyChanged("ExportDirectory"); }
        }

        private String m_config;

        public String ConfigFile
        {
            get { return m_config; }
            set { m_config = value; OnPropertyChanged("ConfigFile"); }
        }

        private IScriptExporter m_exporter;

        public IScriptExporter Exporter
        {
            get { return m_exporter; }
            set { m_exporter = value; OnPropertyChanged("Exporter"); }
        }

        private App m_app;

        public App App
        {
            get { return m_app; }
            set { m_app = value; }
        }

        public NewProjectData(App app)
        {
            m_app = app;
        }

        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
