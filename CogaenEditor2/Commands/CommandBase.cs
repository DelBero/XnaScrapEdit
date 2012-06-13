using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CogaenEditor2.Commands
{
    public abstract class CommandBase : ICommand
    {
        #region member
        private String m_iconName;
        private String m_iconNameSmall;

        
        private String m_name;

        public String IconName
        {
            get { return m_iconName; }
            set { m_iconName = value; }
        }

        public String IconNameSmall
        {
            get { return m_iconNameSmall; }
            set { m_iconNameSmall = value; }
        }

        public String CommandName
        {
            get { return m_name; }
            set { m_name = value; }
        }
        #endregion

        public CommandBase(String commandName, String iconName, String iconNameSmall)
        {
            m_name = commandName;
            m_iconName = iconName;
            m_iconNameSmall = iconNameSmall;
        }

        public override string ToString()
        {
            return CommandName;
        }

        public void changed(object sender, EventArgs e) 
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(sender, e);
            }
        }

        #region ICommand Members
        public event EventHandler CanExecuteChanged;
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);

        #endregion
    }
}
