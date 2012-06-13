using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CogaenEditor2.Manager;

namespace CogaenEditor2.Resources.DataTemplates
{
    public partial class ProjectTemplates
    {
        #region drag n drop
        
        #endregion

        #region mouse handler
        public void GameObjectsGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        public void GameObjectsGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        public void GameObjectsGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        public void GameObjectsGrid_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void Project_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (sender is TextBlock)
                {
                    TextBlock tb = sender as TextBlock;
                    if (tb.DataContext is ProjectTemplateFile)
                    {
                        ProjectTemplateFile template = tb.DataContext as ProjectTemplateFile;
                        App app = (App)App.Current;
                        app.loadTemplate(template);
                    }
                }
            }
        }
        #endregion

        #region events
        private void ProjectElement_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox tb = sender as TextBox;
                tb.IsEnabled = false;
            }
        }

        private void ProjectElement_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                TextBox tb = sender as TextBox;
                tb.IsEnabled = false;
            }
        }
        #endregion
    }
}
