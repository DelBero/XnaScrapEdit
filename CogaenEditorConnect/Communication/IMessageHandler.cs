using System;
namespace CogaenEditorConnect.Communication
{
    public interface IMessageHandler
    {
        void answer(string answer, byte type,uint id, Callback callback, object data);
        //void connect(string ip, string port);
        //void connectCallback(string s, byte type, object data);
        //Connection Connection { get; }
        //CogaenDataItems.DataItems.CogaenData Data { get; set; }
        void disconnect();
        void Dispose();
        void downloadDone();
        void downloading(int done, int total);
        bool Running { get; set; }
        //void send(string s, byte type, Callback callback, object data = null);
        //void sendMessage(CogaenDataItems.DataItems.GameMessage message);
        //void sendMessageCallbackRest(string name, byte type, object data);
        void startDownload();
        void stop();
    }
}
