using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using CogaenDataItems.DataItems;
using System.Xml;
using CogaenDataItems.Helper;
using System.Collections.ObjectModel;
using CogaenDataItems.Manager;
//using System.Windows.Forms;
using CogaenEditor2.Helper;
using CogaenEditorConnect.Communication;

namespace CogaenEditor2.Communication
{
    public partial class MessageHandler : IDisposable
    {
        public void connectCallback(String s, byte type, object data)
        {
            if (m_connection.Connected)
            {
                (App.Current as App).updateData();
            }
            else
            {
                m_connection.Connected = false;
                m_connection.Client = null;
                MessageBox.Show(s);
            }
        }

        public void deleteGameObjectCallback(String asw, byte type, object data)
        {

        }

        #region data updates
        public void updateSubsystemDataCallbackRest(String s, byte type, object data)
        {
            XmlNodeList subsystems = getRestData(s);
            foreach (XmlNode node in subsystems)
            {
                if (node.Name == "Subsystem")
                {
                    String name = "";
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        if (attr.Name == "Name")
                        {
                            name = attr.Value;
                        }
                    }
                    if (name != "")
                    {
                        Subsystem newSub = m_data.AddSubsystem(name);
                        updateServiceData(newSub);
                        updateElementData(newSub);
                    }
                }
            }
        }

        public void updateServiceDataCallbackRest(String s, byte type, object data)
        {
            XmlNodeList services = getRestData(s);
            if (data is Subsystem)
            {
                Subsystem subsystem = data as Subsystem;

                foreach (XmlNode node in services)
                {
                    if (node.Name == "Service")
                    {
                        String name = "";
                        String id = "";
                        foreach (XmlAttribute attr in node.Attributes)
                        {
                            if (attr.Name == "Name")
                            {
                                name = attr.Value;
                            }
                            else if (attr.Name == "Id")
                            {
                                id = attr.Value;
                            }
                        }
                        subsystem.Services.Add(new Service(name, id));
                    }
                }
            }
        }

        public void updateElementDataCallbackRest(String s, byte type, object data)
        {
            XmlNodeList elements = getRestData(s);

            try
            {
                if (data is Subsystem)
                {
                    Subsystem subsystem = data as Subsystem;

                    foreach (XmlNode node in elements)
                    {
                        if (node.Name == "Element")
                        {
                            String name = "";
                            String id = "";
                            foreach (XmlAttribute attr in node.Attributes)
                            {
                                if (attr.Name == "Name")
                                {
                                    name = attr.Value;
                                }
                                else if (attr.Name == "Id")
                                {
                                    id = attr.Value;
                                }
                            }
                            Element comp = new Element(name, id);
                            // get semantic
                            comp.Semantic = DataItemsTools.getElementSemantic(id);
                            subsystem.Elements.Add(comp);
                            updateParameterData(comp);
                            updateMessageData(comp);
                        }
                    }
                    ObservableCollection<Element> elementList = new ObservableCollection<Element>();
                    m_data.getAllComponents(elementList);
                    App.Data.ElementsList = elementList;
                }

                Data.getAllComponents(App.Data.ElementsList);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while loading ElementData: " + e.Message, "Error");
            }
        }

        public void updateParameterDataCallbackRest(String s, byte type, object data)
        {
            XmlNodeList parameter = getRestData(s);
            if (data is Element)
            {
                Element el = data as Element;
                el.Parameters = XmlTools.parseParameterXmlList(parameter, el);
            }
        }

        public void updateMessageDataCallbackRest(String asw, byte type, object data)
        {
            if (data is Element)
            {
                Element el = data as Element;
                XmlNodeList messages = getRestData(asw);
                foreach (XmlElement message in messages)
                {
                    GameMessage msg = new GameMessage();
                    XmlNode msgIdNode = XmlHelper.getNodeByNameAttribute(message.ChildNodes, "msgId");
                    msg.Name = XmlHelper.getNodeAttributeValue(msgIdNode, "Default");
                    msg.Parameter.Params = XmlTools.parseParameterXmlList(message.ChildNodes, el, msgIdNode);
                    el.Messages.Add(msg);
                }
            }
            else if (data is Service)
            {
                Service service = data as Service;
                XmlNodeList messages = getRestData(asw);
                foreach (XmlElement message in messages)
                {
                    GameMessage msg = new GameMessage();
                    XmlNode msgIdNode = XmlHelper.getNodeByNameAttribute(message.ChildNodes, "msgId");
                    msg.Name = XmlHelper.getNodeAttributeValue(msgIdNode, "Default");
                    msg.Parameter.Params = XmlTools.parseParameterXmlList(message.ChildNodes, service, msgIdNode);
                    service.Messages.Add(msg);
                }
            }
        }

        public void updateMacroDataCallbackRest(String asw, byte type, object data)
        {
            XmlNodeList macros = getRestData(asw);
            Data.Macros.Clear();
            foreach (XmlNode macro in macros)
            {
                Data.AddMacro(macro);
            }
        }

        public void updateLiveDataCallbackRest(String asw, byte type, object data)
        {
            XmlNodeList gameobjects = getRestData(asw);
            Data.LiveGameObjects.clear();
            foreach (XmlNode gameobject in gameobjects)
            {
                try
                {
                    Data.AddLiveGameObject(gameobject);
                }
                catch (Exception e)
                {
                    MessageBox.Show("There was an error: " + e.Message, "Error");
                }
            }
        }
        #endregion

        #region script/macro execution
        public void runScriptOnceCallbackRest(String asw, byte type, object data)
        {
            String scriptName = data as String;
            if (scriptName == null)
                return;
            send("POST /Scripts/" + scriptName + " HTTP/1.1", (byte)Connection.CommandType.C_POST, runScriptOnceCallbackRestCallbackRest, scriptName);
        }

        public void runScriptOnceCallbackRestCallbackRest(String asw, byte type, object data)
        {
            // now delete the script
            String scriptName = data as String;
            if (scriptName == null)
                return;
            send("DELETE /Scripts/" + scriptName + " HTTP/1.1", (byte)Connection.CommandType.C_DELETE, runScriptOnceCallbackRestCallbackRestCallbackRest);
        }

        public void runScriptOnceCallbackRestCallbackRestCallbackRest(String asw, byte type, object data)
        {
            // ok enough with the callbacks
        }

        public void registerScriptCallbackRest(String asw, byte type, object data)
        {

        }

        public void runScriptCallbackRest(String name, byte type, object data)
        {

        }

        public void registerMacroCallbackRest(String asw, byte type, object data)
        {
            if (data is ObjectBuilder)
            {
                ObjectBuilder macro = data as ObjectBuilder;
                if (asw == "1")
                {
                    macro.IsLive = true;
                }

            }
        }

        public void runMacroCallbackRest(String asw, byte type, object data)
        {

        }

        public void sendMessageCallbackRest(String name, byte type, object data)
        {

        }
        #endregion

        #region resources
        public void updateResourcesCallbackRest(String asw, byte type, object data)
        {
            XmlNodeList resources = getRestData(asw);

            foreach (XmlNode resourceType in resources)
            {
                String resourceTypeName = "";
                foreach (XmlAttribute attrib in resourceType.Attributes)
                {
                    if (attrib.Name == "Name")
                        resourceTypeName = attrib.Value;
                }
                ResourceFolder folder = Data.Resources.getFolder(resourceTypeName);
                if (folder == null)
                {
                    folder = Data.Resources.addSubFolder(resourceTypeName);
                    updateResourceFolder(resourceTypeName, folder);
                }
            }
        }

        public void updateResourceFolderCallbackRest(String asw, byte type, object data)
        {
            if (data is ResourceFolder)
            {
                ResourceFolder folder = data as ResourceFolder;
                XmlNodeList resources = getRestData(asw);

                foreach (XmlNode resource in resources)
                {
                    foreach (XmlAttribute attrib in resource.Attributes)
                    {
                        if (attrib.Name == "Name")
                        {
                            AbstractResource res = getResourceTypeFromName(resource.Name, attrib.Value);
                            //res.fromXml(resource, ResourceManager);
                            folder.addResource(res);
                        }
                    }
                }
            }
        }

        private void getResourceCallbackRest(String asw, byte type, object data)
        {
            XmlNodeList meshes = getRestData(asw);
            if (data is AbstractResource)
            {
                AbstractResource resource = data as AbstractResource;
                resource.fromXml(meshes[0], App.ResourceManager);
            }
        }
        #endregion
    }
}
