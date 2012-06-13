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
using Microsoft.Win32;
using CogaenDataItems.Exporter;
using CogaenEditor2.Configuration;

namespace CogaenEditor2.GUI.Windows
{
    /// <summary>
    /// Interaction logic for ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        private SaveFileDialog m_save = new SaveFileDialog();

        private IScriptExporter m_exporter = null;

        public IScriptExporter Exporter
        {
            get { return m_exporter; }
            set { m_exporter = value; }
        }

        public ExportWindow()
        {
            InitializeComponent();
            if (m_exporterCombobox.Items.Count > 0)
            {
                m_exporterCombobox.SelectedIndex = 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            m_exporter = m_exporterCombobox.SelectedItem as IScriptExporter;
            App app = Application.Current as App;
            FolderOption path = app.Config.getOption("Export Path") as FolderOption;
            m_save.InitialDirectory = path.Value;
            bool? result =  m_save.ShowDialog();
            if (result.HasValue && result.Value)
            {
                String filename = m_save.FileName;
                if (!filename.EndsWith(m_exporter.Extension))
                {
                    filename += m_exporter.Extension;
                }
                this.SaveFile.Text = filename;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void m_exporterCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IScriptExporter exp = m_exporterCombobox.SelectedItem as IScriptExporter;
            if (exp != null) 
            {
                m_save.Filter = exp.ExtensionFilter;
            }
        }

    }
}
