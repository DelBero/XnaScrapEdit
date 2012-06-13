using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Collections.ObjectModel;

namespace CogaenDataItems.DataItems
{
    #region support calsses
    public class State : IScriptObject
    {
        #region member
        private String m_name;

        public String Name
        {
            get { return m_name; }
            set 
            { 
                m_name = value;
                OnPropertyChanged("Name");
            }
        }

        private ObservableCollection<Transition> m_transitions = new ObservableCollection<Transition>();

        public ObservableCollection<Transition> Transitions
        {
            get { return m_transitions; }
            set 
            { 
                m_transitions = value;
                OnPropertyChanged("Transitions");
            }
        }

        private Transition m_selectedTransition = null;

        public Transition SelectedTransitions
        {
            get { return m_selectedTransition; }
            set
            {
                m_selectedTransition = value;
                OnPropertyChanged("SelectedTransitions");
            }
        }

        private Point m_dimension = new Point(120, 40);
        public override Point Dimension
        {
            get
            {
                return m_dimension;
            }
            set
            {
                m_dimension = value;
                OnPropertyChanged("Dimension");
            }
        }
        #endregion

        #region CDtors
        public State() { }
        public State(String name)
        {
            m_name = name;
        }
        #endregion

        #region methods
        public bool addTransition(Transition trans)
        {
            // search for transition
            foreach (Transition t in m_transitions)
            {
                if (t == trans)
                    return false;
            }
            m_transitions.Add(trans);
            return true;
        }

        public void removeTransition(Transition trans)
        {
            m_transitions.Remove(trans);
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion

        #region Interface IScriptObject
        public override void serialize(BinaryWriter bw)
        {
            throw new NotImplementedException();
        }

        public override void deserialize(BinaryReader br)
        {
            throw new NotImplementedException();
        }

        public override void serializeToXml(System.Xml.XmlDocument doc, System.Xml.XmlElement parent)
        {
            throw new NotImplementedException();
        }

        public override void deserializeFromXml(System.Xml.XmlElement parent)
        {
            throw new NotImplementedException();
        }
        #endregion

        
    }

    public class Transition : IScriptObject
    {
        #region member
        private State m_from;
        public State From
        {
            get { return m_from; }
            set
            {
                m_from = value;
                OnPropertyChanged("From");
            }
        }
        private State m_to;
        public State To
        {
            get { return m_to; }
            set 
            { 
                m_to = value;
                OnPropertyChanged("To");
            }
        }

        public override Point Dimension
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region CDtors
        public Transition() { }
        public Transition(State from, State to)
        {
            m_from = from;
            m_to = to;
        }
        #endregion

        #region methods
        //public static bool operator==(Transition lhs, Transition rhs)
        //{
        //    if (lhs == null && rhs == null)
        //        return true;
        //    else if (lhs == null || rhs == null)
        //        return false;
        //    else
        //        return (lhs.To == rhs.To && lhs.From == rhs.From);
        //}

        //public static bool operator !=(Transition lhs, Transition rhs)
        //{
        //    if (lhs == null && rhs == null)
        //        return false;
        //    else if (lhs == null || rhs == null)
        //        return true;
        //    else
        //        return (lhs.To != rhs.To || lhs.From != rhs.From);
        //}

        public override bool Equals(object obj)
        {
            if (obj is Transition)
            {
                Transition rhs = obj as Transition;
                if (rhs != null)
                    return (this.To == rhs.To && this.From == rhs.From);
                else
                    return false;
            }
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return m_to.ToString();
        }
        #endregion

        #region Interface IScriptObject
        public override void serialize(BinaryWriter bw)
        {
            throw new NotImplementedException();
        }

        public override void deserialize(BinaryReader br)
        {
            throw new NotImplementedException();
        }

        public override void serializeToXml(System.Xml.XmlDocument doc, System.Xml.XmlElement parent)
        {
            throw new NotImplementedException();
        }

        public override void deserializeFromXml(System.Xml.XmlElement parent)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    #endregion
    [Serializable]
    public class StateMachine : IScriptObject
    {
        #region member
        private String m_name;

        public String Name
        {
            get { return m_name; }
            set 
            { 
                m_name = value;
                OnPropertyChanged("Name");
            }
        }

        #region parameters
        private Parameter m_transitionsParameter = new Parameter();

        public static List<String> StateTypes;
        public static String TransitionId;
        public static String MsgId;
        public static String FromId;
        public static String ToId;

        #endregion

        #region mouse input
        private State m_moveState = null;

        public State MoveState
        {
            get { return m_moveState; }
            set 
            { 
                m_moveState = value;
                OnPropertyChanged("MoveState");
            }
        }


        private State m_selectedState = null;

        public State SelectedState
        {
            get { return m_selectedState; }
            set 
            { 
                m_selectedState = value;
                OnPropertyChanged("SelectedState");
            }
        }

        private Point m_start;
        public Point Start
        {
            get { return m_start; }
            set 
            { 
                m_start = value;
                OnPropertyChanged("Start");
            }
        }

        private Point m_end;
        public Point End
        {
            get { return m_end; }
            set 
            { 
                m_end = value;
                OnPropertyChanged("End");
            }
        }

        #endregion


        private ObservableCollection<State> m_states = new ObservableCollection<State>();

        public ObservableCollection<State> States
        {
            get { return m_states; }
            set 
            { 
                m_states = value;
                OnPropertyChanged("States");
            }
        }

        private ObservableCollection<Transition> m_transitions = new ObservableCollection<Transition>();

        public ObservableCollection<Transition> Transitions
        {
            get { return m_transitions; }
            set 
            { 
                m_transitions = value;
                OnPropertyChanged("Transitions");
            }
        }

        private bool m_showStates = false;

        public bool ShowStates
        {
            get { return m_showStates; }
            set 
            { 
                m_showStates = value;
                OnPropertyChanged("ShowStates");
            }
        }

        private Point m_dimension = new Point(120,40);
        public override Point Dimension
        {
            get
            {
                if (ShowStates)
                {
                    return m_dimension;
                }
                else
                {
                    return new Point(120,40);
                }
            }
            set
            {
                m_dimension = value;
                OnPropertyChanged("Dimension");
            }
        }
        #endregion

        #region CDtors
        /// <summary>
        /// Only for use as SampleData in xaml
        /// </summary>
        public StateMachine()
        {

        }

        /// <summary>
        /// Consutrctor to use!
        /// </summary>
        /// <param name="name">Name of the statemachine</param>
        /// <param name="transitions">A list of transitions betweenn the states</param>
        /// <param name="states">The states of the statemachine</param>
        public StateMachine(String name, Parameter transitions, ObservableCollection<Element> states)
        {
            m_name = name;
            m_transitionsParameter = transitions;

            foreach (Element element in states)
            {
                if (element.Semantic == Element.ElementSemantic.STATE)
                {
                    Parameter id = element.getParameter("id");
                    if (id != null)
                    {
                        State newState = new State(id.Values);
                        addState(newState);
                    }
                    
                }
            }
        }
        #endregion

        public void addState(State state)
        {
            m_states.Add(state);
        }

        public void removeState(State state)
        {
            List<Transition> toDelete = new List<Transition>();
            foreach (Transition t in state.Transitions)
            {
                toDelete.Add(t);
                //removeTransition(t);
            }
            foreach (Transition t in toDelete)
            {
                removeTransition(t);
            }
            // remove all Transitions to this state
            toDelete = new List<Transition>();
            foreach (Transition t in m_transitions)
            {
                if (t.To == state)
                {
                    toDelete.Add(t);
                    //removeTransition(t);
                }
            }
            foreach (Transition t in toDelete)
            {
                removeTransition(t);
            }

            m_states.Remove(state);
        }

        public void addTransition(Transition trans)
        {
            int index = m_states.IndexOf(trans.From);
            if (index >= 0)
            {
                if (m_states[index].addTransition(trans))
                    m_transitions.Add(trans);
            }

            // add the transition to the gameobject
            Parameter transition = new Parameter(TransitionId, ParameterType.COMPOUNDPARAMETER, "", 1);
            transition.Params.Add(new Parameter(MsgId, ParameterType.ID, "", 1));
            transition.Params.Add(new Parameter(FromId, ParameterType.ID, trans.From.Name, 1));
            transition.Params.Add(new Parameter(ToId, ParameterType.ID, trans.To.Name, 1));

            m_transitionsParameter.Params.Add(transition);
        }

        public void removeTransition(Transition trans)
        {
            int index = m_states.IndexOf(trans.From);
            if (index >= 0)
            {
                m_states[index].removeTransition(trans);
                m_transitions.Remove(trans);
                // remove from gameobject
                foreach (Parameter p in m_transitionsParameter.Params)
                {
                    if (p.Name == TransitionId)
                    {
                        bool del1 = false;
                        bool del2 = false;
                        foreach (Parameter sub in p.Params)
                        {
                            del1 |= sub.Name == FromId && sub.Values == trans.From.Name;
                            del2 |= sub.Name == ToId && sub.Values == trans.To.Name;
                        }
                        if (del1 && del2)
                        {
                            p.Remove();
                        }
                    }
                }
            }
        }

        #region Interface IScriptObject
        #region (de)serialization
        public override void serialize(BinaryWriter bw)
        {
            throw new NotImplementedException();
        }

        public override void deserialize(BinaryReader br)
        {
            throw new NotImplementedException();
        }

        public override void serializeToXml(System.Xml.XmlDocument doc, System.Xml.XmlElement parent)
        {
            throw new NotImplementedException();
        }

        public override void deserializeFromXml(System.Xml.XmlElement parent)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
