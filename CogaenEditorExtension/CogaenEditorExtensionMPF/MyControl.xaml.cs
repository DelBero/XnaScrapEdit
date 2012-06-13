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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CogaenEditExtension
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {
        public MyControl()
        {
            InitializeComponent();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", this.ToString()),
                            "CogaenEdit");

        }

        private void list_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender != null)
            {
                if (sender is ListBox && (sender as ListBox).SelectedItem != null)
                {
                    ListBox lb = sender as ListBox;

                    try
                    {
                        DragDrop.DoDragDrop(lb, lb.SelectedItem, DragDropEffects.Copy);
                        lb.SelectedIndex = -1;
                    }
                    catch (Exception exp)
                    {
                        Console.Write(exp.Message);
                    }
                }
                //else if (sender is TreeView && (sender as TreeView).SelectedItem != null)
                //{
                //    TreeView tv = sender as TreeView;
                //    DragDrop.DoDragDrop(tv, tv.SelectedItem, DragDropEffects.Copy);
                //}
            }
        }
    }
}