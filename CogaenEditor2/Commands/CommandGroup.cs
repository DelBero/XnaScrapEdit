using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CogaenEditor2.Commands
{
    public class CommandGroup
    {
        private ObservableCollection<CommandList> m_commandGroups = new ObservableCollection<CommandList>();
        private String m_name;

        public ObservableCollection<CommandList> CommandGroups
        {
            get { return m_commandGroups; }
            set { m_commandGroups = value; }
        }

        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public CommandGroup(String name)
        {
            m_name = name;
        }

        public CommandList addCommandList(String name)
        {
            CommandList newList = new CommandList(name);
            m_commandGroups.Add(newList);
            return newList;
        }

        public override string ToString()
        {
            return Name+"ToString";
        }
    }
}
