using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CogaenEditor2.GUI
{
    public interface IHasEntries
    {
        void remove(int entry);

        int SelectedItem
        {
            get;
        }
    }
}
