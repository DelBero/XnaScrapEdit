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
using CogaenEditor._3DEditor;

namespace CogaenEditor.Windows
{
    /// <summary>
    /// Interaction logic for Editor3DWindow.xaml
    /// </summary>
    public partial class Editor3DWindow : Window
    {
        private Editor3D m_editor = null;

        public Editor3D Editor
        {
            get { return m_editor; }
        }

        //public Viewbox Viewbox
        //{
        //    get { return m_viewbox; }
        //}

        public Editor3DWindow(Editor3D editor)
        {
            m_editor = editor;
            DataContext = m_editor;
            InitializeComponent();
        }

        private void test()
        {
            //m_models.Children.Add();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void Viewport3D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m_editor.mouseDown(sender,e);
        }

        private void Viewport3D_MouseMove(object sender, MouseEventArgs e)
        {
            m_editor.mouseMove(sender, e);
        }

        private void Viewport3D_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_editor.mouseUp(sender, e);
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_editor.changeMouseInputMode(comboBox1.SelectedItem as MouseInputMode);
        }

        private void m_viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            m_editor.mouseWheel(e.Delta);
        }

    }
}
