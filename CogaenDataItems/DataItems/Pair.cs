using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CogaenDataItems.Manager
{
    public class Pair<T1, T2>
    {
        private T1 m_key;
        private T2 m_value;

        public T1 Key
        {
            get
            {
                return m_key;
            }
            set
            {
                m_key = value;
            }
        }

        public T2 Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }

        public Pair(T1 key, T2 value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return m_key.ToString();
        }
    }
}
