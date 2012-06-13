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
using CogaenDataItems.DataItems;
using System.Collections.ObjectModel;

namespace CogaenEditorControls.GUI_Elements
{
    /// <summary>
    /// Interaction logic for StateMachineControl.xaml
    /// </summary>
    public partial class StateMachineControl : UserControl
    {
        #region member
        #region commands
        private static RoutedUICommand m_rename = new RoutedUICommand("Rename", "Rename", typeof(StateMachineControl));

        public static RoutedUICommand Rename
        {
            get { return m_rename; }
        }
        #endregion
        private bool m_showLine = false;

        private bool ShowLine
        {
            get { return m_showLine; }
            set
            {
                m_showLine = value;
                if (m_showLine)
                    m_transitionLine.Visibility = System.Windows.Visibility.Visible;
                else
                    m_transitionLine.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private Point m_moveOffset = new Point();
        #endregion
        public StateMachineControl()
        {
            InitializeComponent();
            m_transitionLine.Visibility = System.Windows.Visibility.Hidden;
        }

        #region mouse
        public void StateMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Rectangle)
                {
                    State s = (sender as Rectangle).DataContext as State;
                    StateMachine sm = DataContext as StateMachine;
                    if (sm == null)
                        return;

                    selectState(sm, s);
                    sm.MoveState = null;
                    sm.Start = e.MouseDevice.GetPosition(m_canvas);
                    sm.End = sm.Start;
                    ShowLine = true;
                    e.Handled = true;
                }
            }
        }

        private void StateMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                if (sender is Rectangle)
                {
                    State s = (sender as Rectangle).DataContext as State;
                    StateMachine sm = DataContext as StateMachine;
                    if (sm == null)
                        return;
                    if (sm.SelectedState != null)
                    {
                        // create new Transition
                        Transition trans = new Transition(sm.SelectedState, s);
                        sm.addTransition(trans);
                        selectState(sm, null);
                        ShowLine = false;
                    }
                    e.Handled = true;
                }
            }
        }

        private void Canvas_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StateMachine sm = DataContext as StateMachine;
            if (sm == null)
                return;
            //sm.SelectedState = null;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                StateMachine sm = DataContext as StateMachine;
                if (sm == null)
                    return;
                sm.End = e.MouseDevice.GetPosition(m_canvas);
                // move state?
                if (sm.MoveState != null)
                {
                    Point mousePos = e.MouseDevice.GetPosition(m_canvas);
                    Point newPos = new Point(mousePos.X - m_moveOffset.X, mousePos.Y - m_moveOffset.Y);
                    sm.MoveState.Position = newPos;
                }
            }
        }

        private void GroupBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            StateMachine sm = DataContext as StateMachine;
            if (sm == null)
                return;
            if (sender is GroupBox)
            {
                State state = (sender as GroupBox).DataContext as State;
                selectState(sm, state);
                sm.MoveState = state;
                m_moveOffset = e.MouseDevice.GetPosition(m_canvas);
                m_moveOffset.X -= sm.MoveState.Position.X;
                m_moveOffset.Y -= sm.MoveState.Position.Y;
                ShowLine = false;
            }
            e.Handled = false;
        }

        private void GroupBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StateMachine sm = DataContext as StateMachine;
            if (sm == null)
                return;
            sm.MoveState = null;
            e.Handled = false;
        }

        private void m_transitionList_GotFocus(object sender, RoutedEventArgs e)
        {
            StateMachine sm = DataContext as StateMachine;
            if (sm == null)
                return;
            if (sender is ListView)
            {
                ListView lv = sender as ListView;
                State state = lv.DataContext as State;
                if (state != null)
                {
                    selectState(sm, state);
                }
            }
        }

        private void m_transitionList_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is ListView)
            {
                ListView lv = sender as ListView;
                State state = lv.DataContext as State;
                if (state != null)
                {
                    state.SelectedTransitions = null;
                }
            }
        }

        private void m_transitionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (object obj in e.RemovedItems)
            {
                Transition t = obj as Transition;
                if (t != null)
                {
                    t.Selected = false;
                }
            }

            foreach (object obj in e.AddedItems)
            {
                Transition t = obj as Transition;
                if (t != null)
                {
                    t.Selected = true;
                }
            }
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                if (e.Key == Key.Enter || e.Key == Key.Return)
                    tb.IsEnabled = false;
            }
        }

        #endregion

        #region command bindings
        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StateMachine sm = DataContext as StateMachine;
            if (sm == null)
                return;

            sm.addState(new State("NewState1"));
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StateMachine sm = DataContext as StateMachine;
            if (sm == null)
                return;
            if (e.Parameter is Transition)
            {
                if (sm.SelectedState != null)
                {
                    sm.Transitions.Remove(e.Parameter as Transition);
                    sm.SelectedState.Transitions.Remove(e.Parameter as Transition);
                }
            }
            else if (e.Parameter is State)
            {
                deleteState(sm, e.Parameter as State);
            }
            else if (sm.SelectedState != null && sm.SelectedState.SelectedTransitions != null)
            {
                {
                    sm.Transitions.Remove(sm.SelectedState.SelectedTransitions);
                    sm.SelectedState.Transitions.Remove(sm.SelectedState.SelectedTransitions);
                }
            }
            else if (sm.SelectedState != null)
            {
                deleteState(sm, sm.SelectedState);
            }
        }

        private void Rename_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Rename_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StateMachine sm = DataContext as StateMachine;
            if (sm == null)
                return;

        }
        #endregion

        #region helper
        private void selectState(StateMachine sm, State state)
        {
            if (sm.SelectedState != null)
                sm.SelectedState.Selected = false;
            sm.SelectedState = state;
            if (sm.SelectedState != null)
                sm.SelectedState.Selected = true;
        }

        private void deleteState(StateMachine sm, State state)
        {
            // copy transitions
            ObservableCollection<Transition> transitions = new ObservableCollection<Transition>(state.Transitions);
            // delete transitions from this state
            foreach (Transition t in transitions)
            {
                sm.Transitions.Remove(t);
                state.Transitions.Remove(t);
            }
            // delete transitions to this state
            foreach (State s in sm.States)
            {
                // copy transitions
                ObservableCollection<Transition> transitions2 = new ObservableCollection<Transition>(s.Transitions);
                foreach (Transition t2 in transitions2)
                {
                    if (t2.To == state)
                    {
                        s.Transitions.Remove(t2);
                        sm.Transitions.Remove(t2);
                    }
                }
            }
            // delete state
            sm.States.Remove(state);
        }
        #endregion



    }
}
