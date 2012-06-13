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
using CogaenDataItems.Manager;
using CogaenDataItems.DataItems;
using CogaenEditorControls.Helper;
using CogaenEditorControls.windows;

namespace CogaenEditorControls.GUI_Elements
{
    /// <summary>
    /// Interaction logic for TemplateEditorControl.xaml
    /// </summary>
    public partial class ObjectBuilderControl : UserControl
    {
        #region member
        private bool m_leftDown = false;
        private bool m_middleDown = false;
        private Point m_oldPos = new Point();
        private SelectionBox m_selection = new SelectionBox();

        public bool Dirty
        {
            get
            {
                if (DataContext is IObjectBuilder)
                {
                    return (DataContext as IObjectBuilder).Dirty;
                }
                return false;
            }
            set
            {
                if (DataContext is IObjectBuilder)
                {
                    (DataContext as IObjectBuilder).Dirty = value;
                }
            }
        }

        static List<IScriptObject> m_clipboardObjects = null;

        private bool m_dragging = false;
        #endregion

        public ObjectBuilderControl()
        {
            InitializeComponent();
        }

        private void ResetOffset(object sender, RoutedEventArgs e)
        {
            IObjectBuilder obj = DataContext as IObjectBuilder;
            if (obj != null)
            {
                obj.Offset = new Point(0, 0);
            }
        }

        private void ResetZoom(object sender, RoutedEventArgs e)
        {
            IObjectBuilder obj = DataContext as IObjectBuilder;
            if (obj != null)
            {
                obj.Scaling = 1.0f;
            }
        }

        private void SortSquare(object sender, RoutedEventArgs e)
        {
            IObjectBuilder obj = DataContext as IObjectBuilder;
            if (obj != null)
            {
                obj.sort();
            }
        }

        private void SortList(object sender, RoutedEventArgs e)
        {
            IObjectBuilder obj = DataContext as IObjectBuilder;
            if (obj != null)
            {
                obj.sortTopDown();
            }
        }

        #region drag n drop
        public void GameObject_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (sender is Border)
            {
            }
        }

        public void GameObject_Drop(object sender, DragEventArgs e)
        {
            if (sender is Border)
            {
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
        }

        public void GameObjectsGrid_Drop(object sender, DragEventArgs e)
        {
            Element element = e.Data.GetData(typeof(Element)) as Element;
            if (element != null)
            {
                dropElement(sender, element,e);
            }
        }

        private void dropElement(object sender, Element element, DragEventArgs e)
        {
            IObjectBuilder obj = DataContext as IObjectBuilder;
            if (obj != null)
            {
                IScriptObject so = obj.click(e.GetPosition(sender as Canvas), false,false) as IScriptObject;
                Element newElement = new Element(element);
                if (so != null && so is GameObject)
                {
                    GameObject go = so as GameObject;
                    go.Add(newElement);
                }
                else if (so != null)
                {

                }
                else
                {
                    StringQueryItem qry = new StringQueryItem("Enter new GameObject name", "New GameObject", "New");
                    StringQuery q = new StringQuery();
                    q.DataContext = qry;
                    bool? result = q.ShowDialog();
                    if (result.Value)
                    {
                        GameObject go = obj.newGameObject(qry.Text);
                        if (go != null)
                        {
                            go.Position = e.GetPosition(sender as Canvas);
                            go.Add(newElement);
                        }
                    }
                }
            }
        }
        #endregion

        #region mouse handler
        #region GameObjectsGrid
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
                if (DataContext is IObjectBuilder)
                {
                    IObjectBuilder objectBuilder = DataContext as IObjectBuilder;
                    if (objectBuilder.click(e.GetPosition((Canvas)sender), Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl), false) != null)
                    {
                        // we're done
                        return;
                    }
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
            //App app = (App)App.Current;
            if (e.LeftButton == MouseButtonState.Released)
            {
                m_leftDown = false;
                // end selection
                if (m_selection.Selecting)
                {
                    Rect region = m_selection.endSelection();
                    if (DataContext is IObjectBuilder)
                    {
                        IObjectBuilder objectBuilder = DataContext as IObjectBuilder;
                        List<IScriptObject> selected = objectBuilder.select(region);
                    }
                }
                else if (DataContext is IObjectBuilder && !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    IObjectBuilder objectBuilder = DataContext as IObjectBuilder;
                    // clear selection if we didn't move
                    objectBuilder.click(e.GetPosition((Canvas)sender), false, !m_dragging);
                }
                m_dragging = false;
            }
            if (e.MiddleButton == MouseButtonState.Released)
            {
                m_middleDown = false;
            }
            //app.ObjectBuilder.DragedObject = null;
        }

        public void GameObjectsGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DataContext is ObjectBuilder)
            {
                ObjectBuilder objectBuilder = DataContext as ObjectBuilder;
                objectBuilder.Scaling += e.Delta * 0.001f;
            }
        }

        public void GameObjectsGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_leftDown)
            {
                Point moved = e.GetPosition((Canvas)sender);
                Point dist = new Point(moved.X - m_oldPos.X, moved.Y - m_oldPos.Y);
                // move selected gameobject
                if (DataContext is ObjectBuilder)
                {
                    ObjectBuilder objectBuilder = DataContext as ObjectBuilder;
                    m_dragging = objectBuilder.move(dist);
                }
                // alter selection box
                m_selection.extendSelection(moved);
                m_oldPos = e.GetPosition((Canvas)sender);
            }
            if (m_middleDown)
            {
                Point moved = e.GetPosition((Canvas)sender);
                if (DataContext is IObjectBuilder)
                {
                    IObjectBuilder objectBuilder = DataContext as IObjectBuilder;
                    Point newOffset = new Point(objectBuilder.Offset.X + (moved.X - m_oldPos.X), objectBuilder.Offset.Y + (moved.Y - m_oldPos.Y));
                    objectBuilder.Offset = newOffset;
                }
                m_oldPos = e.GetPosition((Canvas)sender);
            }
        }
        #endregion
        #region component list
        public void list_MouseMove(object sender, MouseEventArgs e)
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

        #region events
        public delegate void DeleteObjectEventHandler(object sender, List<IScriptObject> go);
        public event DeleteObjectEventHandler ObjectDeleted;

        protected void OnObjectDeleted(List<IScriptObject> go)
        {
            DeleteObjectEventHandler handler = ObjectDeleted;
            if (handler != null)
            {
                handler(this, go);
            }
        }
        #endregion


        #region commands
        private void Delete_CanExecute_GameObject(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.DataContext is IObjectBuilder)
            {
                IObjectBuilder objectBuilder = this.DataContext as IObjectBuilder;
                e.CanExecute = objectBuilder.ActiveObject.Count > 0;
            }
            else
                e.CanExecute = false;
        }

        private void Delete_Executed_GameObject(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.DataContext is IObjectBuilder)
            {
                IObjectBuilder objectBuilder = this.DataContext as IObjectBuilder;
                if (objectBuilder.ActiveObject.Count > 0)
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to delete this Gameobjects?", "Are you sure", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        OnObjectDeleted(objectBuilder.ActiveObject);
                        objectBuilder.deleteScriptObject(objectBuilder.ActiveObject);
                    }
                }
            }
        }

        private void Cut_CanExecute_GameObject(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.DataContext is IObjectBuilder)
            {
                IObjectBuilder objectBuilder = this.DataContext as IObjectBuilder;
                if (objectBuilder.ActiveObject.Count > 0)
                    e.CanExecute = true;
            }
            else
                e.CanExecute = false;
        }

        private void Cut_Executed_GameObject(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.DataContext is IObjectBuilder)
            {
                IObjectBuilder objectBuilder = this.DataContext as IObjectBuilder;
                m_clipboardObjects = objectBuilder.ActiveObject;
                objectBuilder.deleteScriptObject(objectBuilder.ActiveObject);
                //Clipboard.SetDataObject(new DataObject("CogaenEditGameObjectList",objectBuilder.ActiveObject));
            }
        }

        private void Copy_CanExecute_GameObject(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.DataContext is IObjectBuilder)
            {
                IObjectBuilder objectBuilder = this.DataContext as IObjectBuilder;
                if (objectBuilder.ActiveObject.Count > 0)
                    e.CanExecute = true;
            }
            else
                e.CanExecute = false;
        }

        private void Copy_Executed_GameObject(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.DataContext is IObjectBuilder)
            {
                IObjectBuilder objectBuilder = this.DataContext as IObjectBuilder;
                m_clipboardObjects = objectBuilder.ActiveObject;
                //Clipboard.SetDataObject(new DataObject(typeof(List<IScriptObject>),objectBuilder.ActiveObject));
            }
        }

        private void Paste_CanExecute_GameObject(object sender, CanExecuteRoutedEventArgs e)
        {
            //IDataObject dataobject = Clipboard.GetDataObject();
            //if (dataobject != null && dataobject.GetData("CogaenEditGameObjectList") is List<IScriptObject>)
            //    e.CanExecute = true;
            //else
            //    e.CanExecute = false;
            e.CanExecute = m_clipboardObjects != null;
        }

        private void Paste_Executed_GameObject(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.DataContext is IObjectBuilder)
            {
                IObjectBuilder objectBuilder = this.DataContext as IObjectBuilder;
                //IDataObject dataobject = Clipboard.GetDataObject();
                //if (dataobject != null && dataobject.GetData(typeof(List<IScriptObject>)) is List<IScriptObject>)
                //{
                //    List<IScriptObject> scriptOjbects = dataobject.GetData(typeof(List<IScriptObject>)) as List<IScriptObject>;
                //    objectBuilder.addScriptObjectCopy(scriptOjbects);
                //}

                objectBuilder.addScriptObjectCopy(m_clipboardObjects);
            }
        }
        #endregion

        private static void AddParameterToMacro(IObjectBuilder objectBuilder)
        {
            Parameter p = new Parameter();
            ParameterWindow paramWin = new ParameterWindow();
            paramWin.DataContext = p;
            bool? ret = paramWin.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                objectBuilder.AddParameter(p);

                // TODO
                // check all macrocalls and update parameters
            }
        }

        private static void RemoveParameterFromMacro(Parameter p)
        {
            p.Remove();
        }

        private void Button_AddParameter(object sender, RoutedEventArgs e)
        {
            ObjectBuilderControl.AddParameterButtonClicked(sender, e);
        }

        public static void AddParameterButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement)
            {
                FrameworkElement fwe = sender as FrameworkElement;
                if (fwe.DataContext is IObjectBuilder)
                {
                    IObjectBuilder obj = fwe.DataContext as IObjectBuilder;
                    AddParameterToMacro(obj);
                }
            }
        }

        public static void RemoveParameterButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement)
            {
                FrameworkElement fwe = sender as FrameworkElement;
                if (fwe.DataContext is Parameter)
                {
                    Parameter p = fwe.DataContext as Parameter;
                    RemoveParameterFromMacro(p);
                }
            }
        }

    }
}
