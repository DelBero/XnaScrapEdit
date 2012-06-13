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
using Microsoft.Windows.Controls.Ribbon;
using CogaenEditor2.Manager;
using CogaenEditor2.GUI;
using CogaenDataItems.DataItems;
using CogaenEditor2.Helper;
using CogaenEditor2.GUI.Windows;
using CogaenEditorControls.GUI_Elements;
using CogaenDataItems.Manager;

namespace CogaenEditor2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        #region member
        private static App m_app;

        public static App Main
        {
            get { return m_app; }
            set { m_app = value; }
        }

        private bool m_leftDown = false;
        private bool m_middleDown = false;
        private Point m_oldPos = new Point();
        private SelectionBox m_selection = new SelectionBox();
        #endregion

        #region CDtors
        public MainWindow()
        {
            Main = App.Current as App;
            InitializeComponent();

            // tell app we are done

            Main.mainWindowDone();
        }
        #endregion

        #region drag n drop
        #region Project
        public void Project_PreviewDragOver(object sender, DragEventArgs e)
        {
            Main.DragDropHandler.Project_PreviewDragOver(sender, e);
        }

        public void Project_Drop(object sender, DragEventArgs e)
        {
            Main.DragDropHandler.Project_Drop(sender, e);
        }
        #endregion
        #region 2DLiveEditor
        private void LiveEditor2D_DragEnter(object sender, DragEventArgs e)
        {
            App app = App.Current as App;
            app.DragDropHandler.GameObjectsGrid_DragEnter(sender, e);
        }

        private void LiveEditor2D_Drop(object sender, DragEventArgs e)
        {
            App app = App.Current as App;
            app.DragDropHandler.GameObjectsGrid_Drop(sender, e);
        }
        #endregion
        #endregion

        #region events
        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App app = (App)App.Current;
            if (app.canClose())
            {
                app.Shutdown();
                app.Dispose();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //App app = (App)App.Current;
            //if (this.Ribbon.SelectedIndex == 2) // 3D Editor
            //{
            //    m_MainTab.SelectedIndex = 1;
            //}
            //else
            //{
            //    m_MainTab.SelectedIndex = 0;
            //}
            //app.ribbonChanged(e);
        }

        private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            App app = (App)App.Current;
            app.MessageHandler.updateMacroData();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App app = (App)App.Current;
            app.selectObjectBuilder(this.TemplateTab.SelectedIndex);
        }

        private void m_MainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_MainTab.SelectedItem == m_2dLiveEditor)
            {
                Main.MessageHandler.updateLiveGameobjectData();
                Main.selectLiveEditor();
                Main.ObjectBuilder.sort();
            }
            else
            {
                Main.selectTemplateEditor();
            }
        }

        private void RegisterMacro_Click(object sender, RoutedEventArgs e)
        {
            if (m_MacroList.SelectedItem != null)
            {

                if (m_MacroList.SelectedItem is ObjectBuilder)
                {
                    App app = (App)App.Current;
                    //app.registerMacroInScript(m_MacroList.SelectedItem as ObjectBuilder);
                    ObjectBuilder obj = m_MacroList.SelectedItem as ObjectBuilder;
                    if (obj != null)
                    {
                        StringQueryItem qry = new StringQueryItem("Enter macro name", "Macro Registration", "Macro");
                        StringQuery q = new StringQuery();
                        q.DataContext = qry;
                        bool? result = q.ShowDialog();
                        if (result.Value)
                        {
                            if (result.Value)
                            {
                                Main.MessageHandler.registerMacro(qry.Text, obj);
                            }
                        }

                    }
                    sortMacros();
                }
            }
        }
        #endregion

        #region mouse handling
        #region project
        private void Project_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed && sender != null)
            {
                if (sender is TreeView && (sender as TreeView).SelectedItem != null)
                {
                    TreeView tv = sender as TreeView;

                    try
                    {
                        DragDrop.DoDragDrop(tv, tv.SelectedItem, DragDropEffects.Move);
                    }
                    catch (Exception exp)
                    {

                        Console.Write(exp.Message);
                    }
                }
            }
        }
        #endregion
        #region 2DLiveEditor
        private void LiveEditor2D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App app = App.Current as App;
            //GameObjectsGrid_MouseDown(sender, e);
            if (sender is UIElement)
            {
                UIElement uiElement = sender as UIElement;
                uiElement.Focus();
            }
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    m_leftDown = true;
                }
                m_oldPos = e.GetPosition((Canvas)sender);
                if (Main.ObjectBuilder.click(e.GetPosition((Canvas)sender)) != null)
                {
                    // we're done
                    return;
                }
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    // start selecting
                    m_selection.startSelection(m_oldPos, sender as Canvas);
                }
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                m_oldPos = e.GetPosition((Canvas)sender);
                m_middleDown = true;
            }
        }

        private void LiveEditor2D_MouseMove(object sender, MouseEventArgs e)
        {
            App app = App.Current as App;
            //GameObjectsGrid_MouseMove(sender, e);
            if (m_leftDown)
            {
                Point moved = e.GetPosition((Canvas)sender);
                Point dist = new Point(moved.X - m_oldPos.X, moved.Y - m_oldPos.Y);
                // move selected gameobject
                Main.ObjectBuilder.move(dist);
                // alter selection box
                m_selection.extendSelection(moved);
                m_oldPos = e.GetPosition((Canvas)sender);
            }
            if (m_middleDown)
            {
                Point moved = e.GetPosition((Canvas)sender);
                Point newOffset = new Point(Main.ObjectBuilder.Offset.X + (moved.X - m_oldPos.X), Main.ObjectBuilder.Offset.Y + (moved.Y - m_oldPos.Y));
                Main.ObjectBuilder.Offset = newOffset;
                m_oldPos = e.GetPosition((Canvas)sender);
            }

        }

        private void LiveEditor2D_MouseUp(object sender, MouseButtonEventArgs e)
        {
            App app = App.Current as App;
            //GameObjectsGrid_MouseUp(sender, e);
            if (e.LeftButton == MouseButtonState.Released)
            {
                m_leftDown = false;
                // end selection
                if (m_selection.Selecting)
                {
                    Rect region = m_selection.endSelection();
                    List<IScriptObject> selected = Main.ObjectBuilder.select(region);
                }
            }
            if (e.MiddleButton == MouseButtonState.Released)
            {
                m_middleDown = false;
            }
            app.ObjectBuilder.DragedObject = null;
        }

        private void LiveEditor2D_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            App app = App.Current as App;
            app.ObjectBuilder.Scaling += e.Delta * 0.001f;
        }
        #endregion
        #region list
        private void list_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //StartDrag(e);
            }
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
                    }
                    catch (Exception exp)
                    {

                        Console.Write(exp.Message);
                    }
                }
                else if (sender is TreeView && (sender as TreeView).SelectedItem != null)
                {
                    TreeView tv = sender as TreeView;
                    DragDrop.DoDragDrop(tv, tv.SelectedItem, DragDropEffects.Copy);
                }
            }
        }
        #endregion
        #endregion

        public void sortMacros()
        {
            throw new NotImplementedException();
        }
    }
}
