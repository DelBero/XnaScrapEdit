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
using CogaenDataItems.DataItems;
using CogaenEditor2.Helper;

namespace CogaenEditor2.GUI.Windows
{
    /// <summary>
    /// Interaction logic for StateMachineWindow.xaml
    /// </summary>
    public partial class StateMachineWindow : Window
    {
        public StateMachineWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        /*
        #region mouse
        public void StateMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Rectangle)
                {
                    State s = (sender as Rectangle).DataContext as State;
                    StateMachine sm = DataContext as StateMachine;

                    sm.SelectedState = s;
                    sm.MoveState = null;
                    sm.Start = e.MouseDevice.GetPosition(m_canvas);
                    sm.End = sm.Start;
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
                    if (sm.SelectedState != null)
                    {
                        // create new Transition
                        Transition trans = new Transition(sm.SelectedState, s);
                        sm.addTransition(trans);
                        sm.SelectedState = null;
                    }
                    e.Handled = true;
                }
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                StateMachine sm = DataContext as StateMachine;
                //sm.SelectedState = null;
            }
        }

        private void m_canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StateMachine sm = DataContext as StateMachine;
            sm.SelectedState = null;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                StateMachine sm = DataContext as StateMachine;
                sm.End = e.MouseDevice.GetPosition(m_canvas);
                // move state?
                if (sm.MoveState != null)
                {
                    sm.MoveState.Position = e.MouseDevice.GetPosition(m_canvas);
                }
            }
        }

        private void GroupBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StateMachine sm = DataContext as StateMachine;
            if (sender is GroupBox)
            {
                State state = (sender as GroupBox).DataContext as State;
                sm.MoveState = state;
            }
        }

        private void GroupBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StateMachine sm = DataContext as StateMachine;
            sm.MoveState = null;
        }
        #endregion
        */
        #region commandbinding
        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StateMachine sm = DataContext as StateMachine;
            if (e.Parameter is Transition)
            {
                sm.removeTransition(e.Parameter as Transition);
            }
            else if (e.Parameter is State)
            {
                sm.removeState(e.Parameter as State);
            }
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StateMachine sm = DataContext as StateMachine;
            if (e.Parameter is StateMachine)
            {
                StringQueryItem sqi = new StringQueryItem("Enter new state's name", "New State");
                StringQuery sq = new StringQuery();
                sq.DataContext = sqi;
                bool? result = sq.ShowDialog();
                if (result.Value)
                {
                    State state = new State(sqi.Text);
                    sm.addState(state);
                }
            }

        }

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

    }
}
