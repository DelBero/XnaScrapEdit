using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CogaenEditor2.Commands
{
    public class IconCommand
    {
        private String m_name = "";

        public String Name
        {
            get { return m_name; }
        }


        private ICommand m_command = null;

        public ICommand Command
        {
            get { return m_command; }
        }

        private String m_iconSource = "";

        public String IconSource
        {
            get { return m_iconSource; }
        }
        private String m_smallIconSource = "";

        public String SmallIconSource
        {
            get { return m_smallIconSource; }
        }

        public IconCommand(String name, ICommand command, String icon, String iconSmall)
        {
            m_name = name;
            m_command = command;
            m_iconSource = icon;
            m_smallIconSource = iconSmall;
        }
    }

    public class CommandList
    {
        private ObservableCollection<IconCommand> m_commands = new ObservableCollection<IconCommand>();
        private String m_name;

        public String GroupName
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public ObservableCollection<IconCommand> Commands
        {
            get { return m_commands; }
            set { m_commands = value; }
        }

        public CommandList(String name)
        {
            m_name = name;
        }

        public void addCommand(String name, ICommand command, String icon, String iconSmall)
        {
            m_commands.Add(new IconCommand(name, command, icon, iconSmall));
        }
    }
}
