using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;
using CogaenDataItems.DataItems;

namespace CogaenDataItems.Helper
{
    public class XmlHelper
    {
        /// <summary>
        /// Returns the node that has a attribute 'Name' with value 'name'
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="name">the name to look for</param>
        /// <returns></returns>
        public static XmlNode getNodeByNameAttribute(XmlNodeList nodeList, string name)
        {
            return getNodeByAttribute(nodeList,"Name", name);
        }

        /// <summary>
        /// Return node with name 'name'
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XmlNode getNodeByName(XmlNodeList nodeList, string name)
        {
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == name)
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the node that has a attribute 'attribName' with value 'value'.
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="attribName">the attribute to look for</param>
        /// <param name="value">the value to look for</param>
        /// <returns></returns>
        public static XmlNode getNodeByAttribute(XmlNodeList nodeList, string attribName, string value)
        {
            string attribNameLower = attribName.ToLower();
            string valueLower = value.ToLower();
            foreach (XmlNode node in nodeList)
            {
                foreach (XmlAttribute attrib in node.Attributes)
                {
                    if (attrib.Name.ToLower() == attribNameLower && attrib.Value.ToLower() == valueLower)
                        return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Return the value of attribute 'attribName'
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attribName"></param>
        /// <returns></returns>
        public static String getNodeAttributeValue(XmlNode node, string attribName)
        {
            foreach (XmlAttribute attrib in node.Attributes)
            {
                if (attrib.Name == attribName)
                    return attrib.Value;
            }
            return null;
        }

        /// <summary>
        /// Find the first node that has a attribute 'Name' and return it's value.
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static String getNodeValueByNameAttribute(XmlNodeList nodeList, string name, out XmlNode node)
        {
            node = null;
            XmlNode n = getNodeByNameAttribute(nodeList, name);
            if (n != null)
            {
                node = n;
                return n.Attributes["Name"].Value;
            }
            return null;
        }

    }
}
