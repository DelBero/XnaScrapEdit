using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CogaenEditorConnect.Communication
{
    public interface IConnectionLogOutput
    {
        void OutputConnectionLogMessage(string message);
    }

    public class ConnectionLog
    {
        private static ConnectionLog m_instance = new ConnectionLog();
        public static ConnectionLog Instance
        {
            get { return m_instance; }
        }

        public const uint MaxMessages = 400;

        public enum LogMessageType
        {
            Error,
            Warning,
            Message,
            Answer
        }

        private List<LogMessage> m_messages = new List<LogMessage>();

        public List<LogMessage> Messages
        {
            get { return m_messages; }
        }

        private List<IConnectionLogOutput> m_output = new List<IConnectionLogOutput>();

        private ConnectionLog()
        {

        }

        public static void RegisterOutput(IConnectionLogOutput output)
        {
            if (!Instance.m_output.Contains(output))
                Instance.m_output.Add(output);
        }

        public static void RemoveOutput(IConnectionLogOutput output)
        {
            if (Instance.m_output.Contains(output))
                Instance.m_output.Remove(output);
        }

        public static void LogError(uint id, string error)
        {
            beforeLogging();
            Instance.Messages.Add(new LogMessage(LogMessageType.Error, id, error));
            afterLogging();
        }

        public static void LogWarning(uint id, string warning)
        {
            beforeLogging();
            Instance.Messages.Add(new LogMessage(LogMessageType.Warning, id, warning));
            afterLogging();
        }

        public static void LogMessage(uint id, string msg)
        {
            beforeLogging();
            Instance.Messages.Add(new LogMessage(LogMessageType.Message, id, msg));
            afterLogging();
        }

        public static void LogAnswer(uint id, string asw)
        {
            beforeLogging();
            Instance.Messages.Add(new LogMessage(LogMessageType.Answer, id, asw));
            afterLogging();
        }

        private static void beforeLogging()
        {
            if (Instance.Messages.Count >= MaxMessages)
            {
                Instance.Messages.RemoveAt(0);
            }
        }

        private static void afterLogging()
        {
            foreach (IConnectionLogOutput output in Instance.m_output)
            {
                output.OutputConnectionLogMessage(Instance.Messages.Last().ToString());
            }
        }
    }

    public class LogMessage
    {
        private ConnectionLog.LogMessageType m_type;

        public ConnectionLog.LogMessageType Type
        {
            get { return m_type; }
        }

        private String m_message;

        public String Message
        {
            get { return m_message; }
        }

        private uint m_id = 0;

        public uint Id
        {
            get { return m_id; }
        }

        public LogMessage(ConnectionLog.LogMessageType type, uint id, String message)
        {
            m_type = type;
            m_message = message;
            m_id = id;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<");
            if (Id > 0)
            {
                 sb.Append(Id.ToString()); sb.Append(" ");
            }
            sb.Append(Enum.GetName(typeof(ConnectionLog.LogMessageType), m_type)); sb.Append(">");
            sb.Append(Message);
            return sb.ToString();
        }
    }
}
