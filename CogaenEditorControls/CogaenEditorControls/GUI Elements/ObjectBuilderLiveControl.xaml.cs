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
    public partial class ObjectBuilderLiveControl : UserControl
    {
        #region member
        protected bool m_leftDown = false;
        protected bool m_middleDown = false;
        protected Point m_oldPos = new Point();
        protected SelectionBox m_selection = new SelectionBox();
        private CogaenData m_liveData = null;

        protected CogaenData LiveData
        {
            get { return m_liveData; }
            set { m_liveData = value; }
        }

        public virtual bool Dirty
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


        private bool m_autoUpdate;

        public bool AutoUpdate
        {
            get { return m_autoUpdate; }
            set { m_autoUpdate = value; }
        }

        private ushort m_updateTimer = 5;

        public ushort UpdateTimer
        {
            get { return m_updateTimer; }
            set { m_updateTimer = value; }
        }

        protected List<IScriptObject> m_clipboardObjects = null;

        protected bool m_dragging = false;
        #endregion

        #region events
        public delegate void RequestLiveGameobjectUpdateEventHandler(object sender);
        public event RequestLiveGameobjectUpdateEventHandler RequestLiveGameobjectUpdate;

        public delegate void RequestLiveGameobjectPropertiesEventHandler(LiveGameObject gameobject);
        public event RequestLiveGameobjectPropertiesEventHandler RequestLiveGameobjectProperties;

        public delegate void GameObjectSelectedEventHandler(object sender, IList<IScriptObject> selection);
        public event GameObjectSelectedEventHandler GameObjectSelectionChanged;

        private void OnSelectionChanged(IList<IScriptObject> selection)
        {
            if (selection == null)
                selection = new List<IScriptObject>();

            if (GameObjectSelectionChanged != null)
            {
                GameObjectSelectionChanged(this, selection);
            }
        }
        #endregion

        public ObjectBuilderLiveControl()
        {
            InitializeComponent();
            this.Focusable = true;
        }


        protected virtual void ResetOffset(object sender, RoutedEventArgs e)
        {
            IObjectBuilder obj = DataContext as IObjectBuilder;
            if (obj != null)
            {
                obj.Offset = new Point(0, 0);
            }
        }

        protected virtual void ResetZoom(object sender, RoutedEventArgs e)
        {
            IObjectBuilder obj = DataContext as IObjectBuilder;
            if (obj != null)
            {
                obj.Scaling = 1.0f;
            }
        }

        protected virtual void SortSquare(object sender, RoutedEventArgs e)
        {
            IObjectBuilder obj = DataContext as IObjectBuilder;
            if (obj != null)
            {
                obj.sort();
            }
        }

        protected virtual void SortList(object sender, RoutedEventArgs e)
        {
            IObjectBuilder obj = DataContext as IObjectBuilder;
            if (obj != null)
            {
                obj.sortTopDown();
            }
        }
        
        #region mouse handler
        #region GameObjectsGrid
        public virtual void GameObjectsGrid_MouseDown(object sender, MouseButtonEventArgs e)
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

        public virtual void GameObjectsGrid_MouseUp(object sender, MouseButtonEventArgs e)
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
                        objectBuilder.select(region);
                        OnSelectionChanged(objectBuilder.ActiveObject);
                    }
                }
                else if (DataContext is IObjectBuilder && !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    IObjectBuilder objectBuilder = DataContext as IObjectBuilder;
                    // clear selection if we didn't move
                    objectBuilder.click(e.GetPosition((Canvas)sender), false, !m_dragging);
                    OnSelectionChanged(objectBuilder.ActiveObject);
                }
                m_dragging = false;
            }
            if (e.MiddleButton == MouseButtonState.Released)
            {
                m_middleDown = false;
            }
            //app.ObjectBuilder.DragedObject = null;
        }

        public virtual void GameObjectsGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DataContext is ObjectBuilder)
            {
                ObjectBuilder objectBuilder = DataContext as ObjectBuilder;
                objectBuilder.Scaling += e.Delta * 0.001f;
            }
        }

        public virtual void GameObjectsGrid_MouseMove(object sender, MouseEventArgs e)
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
        protected virtual void Delete_CanExecute_GameObject(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.DataContext is IObjectBuilder)
            {
                IObjectBuilder objectBuilder = this.DataContext as IObjectBuilder;
                e.CanExecute = objectBuilder.ActiveObject.Count > 0;
            }
            else
                e.CanExecute = false;
        }

        protected virtual void Delete_Executed_GameObject(object sender, ExecutedRoutedEventArgs e)
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

        protected virtual void Copy_CanExecute_GameObject(object sender, CanExecuteRoutedEventArgs e)
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

        protected virtual void Copy_Executed_GameObject(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.DataContext is IObjectBuilder)
            {
                IObjectBuilder objectBuilder = this.DataContext as IObjectBuilder;
                m_clipboardObjects = objectBuilder.ActiveObject;
                //Clipboard.SetDataObject(new DataObject("CogaenEditGameObjectList",objectBuilder.ActiveObject));
            }
        }

        protected virtual void Refresh_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        protected virtual void Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (RequestLiveGameobjectUpdate != null)
            {
                RequestLiveGameobjectUpdate(sender);
            }
            // recompute z-Order
            if (DataContext is IObjectBuilder)
            {
                ((IObjectBuilder)DataContext).RecomputeZOrder();
            }
        }

        protected virtual void Props_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        protected virtual void Props_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (RequestLiveGameobjectProperties != null)
            {
                RequestLiveGameobjectProperties(e.Parameter as LiveGameObject);
            }
        }
        #endregion        

        public void RegisterForRefresh(RequestLiveGameobjectUpdateEventHandler handler)
        {
            if (RequestLiveGameobjectUpdate == null)
            {
                RequestLiveGameobjectUpdate += handler;
            }
        }

        public void RegisterForProperties(RequestLiveGameobjectPropertiesEventHandler handler)
        {
            if (RequestLiveGameobjectProperties == null)
            {
                RequestLiveGameobjectProperties += handler;
            }
        }

        
    }
}
