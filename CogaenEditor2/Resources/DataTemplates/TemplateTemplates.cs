using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CogaenDataItems.DataItems;
using CogaenEditor2.Manager;
using CogaenEditor2.GUI;
using CogaenEditorControls.GUI_Elements;

namespace CogaenEditor2.Resources.DataTemplates
{
    public partial class TemplateTemplates
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
        public TemplateTemplates()
        {
            Main = App.Current as App;
        }
        #endregion

        #region drag n drop
        public void GameObject_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (sender is Border)
            {
                App app = App.Current as App;
                app.DragDropHandler.GameObject_PreviewDragOver((sender as Border).DataContext as GameObject,e);
            }
        }

        public void GameObject_Drop(object sender, DragEventArgs e)
        {
            if (sender is Border)
            {
                App app = App.Current as App;
                app.DragDropHandler.GameObject_Drop((sender as Border).DataContext as GameObject, e);
            }
        }

        public void Parameter_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (sender is TextBox)
            {
                //Parameter.PreviewDragOver((sender as TextBox).DataContext as Parameter, e);
            }
        }

        public void Parameter_Drop(object sender, DragEventArgs e)
        {
            if (sender is TextBox)
            {
                //Parameter.Drop((sender as TextBox).DataContext as Parameter, e);
            }
        }

        public void GameObjectsGrid_DragEnter(object sender, DragEventArgs e)
        {
            App app = App.Current as App;
            app.DragDropHandler.GameObjectsGrid_DragEnter(sender, e);
        }

        public void GameObjectsGrid_Drop(object sender, DragEventArgs e)
        {
            App app = App.Current as App;
            app.DragDropHandler.GameObjectsGrid_Drop(sender, e);
        }
        #endregion

        #region mouse handler
        public void GameObjectsGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
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
            //this.Canvas1.InvalidateVisual();
        }

        public void GameObjectsGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            App app = (App)App.Current;
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

        public void GameObjectsGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Main.ObjectBuilder.Scaling += e.Delta * 0.001f;
        }

        public void GameObjectsGrid_MouseMove(object sender, MouseEventArgs e)
        {
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


        #endregion

        #region events
        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Border border = sender as Border;

            if (border.DataContext is GameObject)
            {
                GameObject go = border.DataContext as GameObject;
                go.Dimension = new Point(e.NewSize.Width, e.NewSize.Height);
            }
            else if (border.DataContext is LiveGameObject)
            {
                LiveGameObject lgo = border.DataContext as LiveGameObject;
                lgo.Dimension = new Point(e.NewSize.Width, e.NewSize.Height);
            }
            else if (border.DataContext is MacroRegistration)
            {
                MacroRegistration mr = border.DataContext as MacroRegistration;
                mr.Dimension = new Point(e.NewSize.Width, e.NewSize.Height);
            }
            else if (border.DataContext is MacroCall)
            {

                MacroCall mc = border.DataContext as MacroCall;
                mc.Dimension = new Point(e.NewSize.Width, e.NewSize.Height);
            }
        }
        #endregion

        #region commands
        private void Delete_CanExecute_GameObject(object sender, CanExecuteRoutedEventArgs e)
        {
            App app = Application.Current as App;
            e.CanExecute = app.ObjectBuilder != null;
        }

        private void Delete_Executed_GameObject(object sender, ExecutedRoutedEventArgs e)
        {
            App app = Application.Current as App;
            app.delete();
        }

        private void Copy_CanExecute_GameObject(object sender, CanExecuteRoutedEventArgs e)
        {
            App app = Application.Current as App;
            e.CanExecute = app.ObjectBuilder != null;
        }

        private void Copy_Executed_GameObject(object sender, ExecutedRoutedEventArgs e)
        {
            App app = Application.Current as App;
            app.copy();
        }

        private void Paste_CanExecute_GameObject(object sender, CanExecuteRoutedEventArgs e)
        {
            App app = Application.Current as App;
            e.CanExecute = app.ObjectBuilder != null;
        }

        private void Paste_Executed_GameObject(object sender, ExecutedRoutedEventArgs e)
        {
            App app = Application.Current as App;
            app.paste();
        }
        #endregion
    }
}
