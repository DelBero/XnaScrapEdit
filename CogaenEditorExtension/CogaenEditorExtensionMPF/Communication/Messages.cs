using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using CogaenDataItems.DataItems;
using CogaenDataItems.Exporter;
using CogaenDataItems.Manager;
using CogaenEditorConnect.Communication;

namespace CogaenEditExtension.Communication
{

    public partial class MessageHandler : IDisposable
    {
        public void deleteGameObject(String name)
        {
            send("DELETE /GameObjects/" + name + " HTTP/1.1", (byte)Connection.CommandType.C_DELETE, deleteGameObjectCallback);
        }

        #region data update
        public void updateSubsystemData()
        {
            send("GET /Subsystems/ HTTP/1.1", (byte)Connection.CommandType.C_GET, updateSubsystemDataCallbackRest);
        }

        public void updateServiceData(Subsystem subsystem)
        {
            send("GET /Services/ HTTP/1.1", (byte)Connection.CommandType.C_GET, updateServiceDataCallbackRest, subsystem);
        }

        public void updateElementData(Subsystem subsystem)
        {
            send("GET /Elements/ HTTP/1.1", (byte)Connection.CommandType.C_GET, updateElementDataCallbackRest, subsystem);
        }

        private void updateParameterData(Element el)
        {
                send("GET /Elements/" + el.Name + "/Parameters/ HTTP/1.1", (byte)Connection.CommandType.C_GET, updateParameterDataCallbackRest, el);
        }

        public void updateMessageData(Element el)
        {
            send("GET /Elements/" + el.Name + "/Messages/ HTTP/1.1", (byte)Connection.CommandType.C_GET, updateMessageDataCallbackRest, el);
        }

        public void updateMacroData()
        {
            send("GET /Macros/ HTTP/1.1", (byte)Connection.CommandType.C_GET, updateMacroDataCallbackRest);
        }

        public void updateLiveGameobjectData()
        {
            send("GET /GameObjects/ HTTP/1.1", (byte)Connection.CommandType.C_GET, updateLiveDataCallbackRest);
        }
        #endregion

        #region script/macro execution
        public void runScriptOnce(String name, IObjectBuilder script)
        {
            throw new NotImplementedException("Implement runScriptOnce and ScriptExporter");
            IScriptExporter exporter = null;// App.CurrentProject.Exporter;
            if (exporter == null)
            {
                MessageBox.Show("No Exporter set", "Error");
                return;
            }
            String scriptExported = script.exportScript(exporter);
            send("PUT /Scripts/" + name + "/ " + scriptExported + " HTTP/1.1", (byte)Connection.CommandType.C_PUT, runScriptOnceCallbackRest, name);
        }

        public void registerScript(String name, ObjectBuilder script)
        {
            throw new NotImplementedException("Implement registerScript and ScriptExporter");
            IScriptExporter exporter = null;// App.CurrentProject.Exporter;
            String scriptExported = script.exportScript(exporter);
            send("PUT /Scripts/" + name + " " + scriptExported + " HTTP/1.1", (byte)Connection.CommandType.C_PUT, registerScriptCallbackRest);
        }

        public void runScript(String name)
        {
            send("POST /Scripts/" + name + " HTTP/1.1", (byte)Connection.CommandType.C_POST, runScriptCallbackRest);
        }

        public void registerMacro(String name, ObjectBuilder macro)
        {
            throw new NotImplementedException("Implement registerMacro and ScriptExporter");
            IScriptExporter exporter = null;// App.CurrentProject.Exporter;
            String macroScript = macro.exportScript(exporter);
            macro.RegisteredName = name;
            send("PUT /Macros/" + name + " " + macroScript, (byte)Connection.CommandType.C_PUT, registerMacroCallbackRest, macro);
        }

        public void runMacro(String name)
        {
            send("POST /Macros/" + name + " HTTP/1.1", (byte)Connection.CommandType.C_POST, runMacroCallbackRest);
        }

        public void sendMessage(GameMessage message)
        {
            String xml = message.toXml(message.Target.Name);
            send("PUT /GameObjects/" + message.Target.Name + "/Messages/ " + xml + " HTTP/1.1", (byte)Connection.CommandType.C_PUT, sendMessageCallbackRest);
        }
        #endregion

        #region resources
        public void updateResources()
        {
            send("GET /Resources/ HTTP/1.1", (byte)Connection.CommandType.C_GET, updateResourcesCallbackRest);
        }

        public void updateResourceFolder(String name, ResourceFolder folder)
        {
            send("GET /Resources/" + name + "/ HTTP/1.1", (byte)Connection.CommandType.C_GET, updateResourceFolderCallbackRest, folder);
        }

        public void getResource(AbstractResource resource)
        {
            send("GET /Resources/" + AbstractResource.ResourceTypeToString(resource.Type) + "/" + resource.Name + " HTTP/1.1", (byte)Connection.CommandType.C_GET, getResourceCallbackRest, resource);
        }
        #endregion
    }
}
