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
using CogaenEditor2.DataItems;
using CogaenEditor2.Config;
using Microsoft.Win32;

namespace CogaenEditor2.Windows
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            TextBox tb =  sender as TextBox;
            CogaenEditor.DataItems.Component component = e.Data.GetData(typeof(CogaenEditor.DataItems.Component)) as CogaenEditor.DataItems.Component;
            Parameter parameter = e.Data.GetData(typeof(Parameter)) as Parameter;
            if (tb != null)
            {
                String newText = "";
                String oldText = tb.Text.Trim();
                if (oldText.Length != 0)
                {
                    newText += oldText + ", ";
                }
                if (component != null)
                {
                    if (!newText.Contains(component.Id))
                    {
                        newText += component.Id;
                    }
                }
                else if (parameter != null)
                {
                    if (!newText.Contains(parameter.Name))
                    {
                        newText += parameter.Name;
                    }
                }
                tb.Text = newText;
            }
        }

        private void TextBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            ListBox lb = sender as ListBox;
            CogaenEditor.DataItems.Component component = e.Data.GetData(typeof(CogaenEditor.DataItems.Component)) as CogaenEditor.DataItems.Component;
            Parameter parameter = e.Data.GetData(typeof(Parameter)) as Parameter;
            if (lb != null)
            {
                if (component != null)
                {
                    object o = lb.FindName(component.Id);
                    if (o == null)
                    {
                        ListOption lo = (lb.DataContext as ListOption);
                        if (lo != null)
                        {
                            lo.Value.Add(component.Id);
                        }
                        //lb.Items.Add(component.Id);
                    }
                }
            }
        }

        private void ListBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        /// <summary>
        /// Browse button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                FolderOption fo = button.DataContext as FolderOption;
                if (fo != null)
                {
                    System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                    fbd.ShowNewFolderButton = true;
                    fbd.SelectedPath = fo.Value;
                    System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        fo.Value = fbd.SelectedPath.ToString();
                    }
                }
            }
        }

    }
}
