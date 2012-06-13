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
using System.Collections.Specialized;

namespace CogaenEditor
{
    /// <summary>
    /// Interaction logic for ListWindow.xaml
    /// </summary>
    public partial class ListWindow : Window
    {
        public ListWindow()
        {
            InitializeComponent();            
        }

        public void Show(object list)
        {
            if (list is INotifyCollectionChanged)
            {
                this.DataContext = list;
            }
            this.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            this.Hide();
        }

        private void listView1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //StartDrag(e);
            }
        }

        private void StartDrag(MouseButtonEventArgs e)
        {
            DragDrop.DoDragDrop(this.listView1,this.listView1.SelectedItem, DragDropEffects.Copy);
        }

        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && this.listView1.SelectedItem != null)
            {
                DragDrop.DoDragDrop(this.listView1, this.listView1.SelectedItem, DragDropEffects.Copy);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
