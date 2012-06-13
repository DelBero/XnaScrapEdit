using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CogaenDataItems.Helper;
using System.Collections.ObjectModel;
using CogaenDataItems.DataItems;
using System.Xml;

namespace CogaenEditExtension.Helper
{
    public partial class XmlTools: XmlHelper
    {
        public static ObservableCollection<Parameter> parseParameterXmlList(XmlNodeList parameterData, IParameterContainer component, XmlNode skip)
        {
            ObservableCollection<Parameter> parameterList = new ObservableCollection<Parameter>();
            foreach (XmlNode node in parameterData)
            {
                if (node == skip)
                    continue;

                Parameter p = new Parameter();
                if (component is Element)
                    p.Semantic = DataItemsTools.getParameterSemantic(node, component as Element, p);
                else
                    p.Semantic = ParameterSemantic.NONE;

                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.Name == "Name")
                    {
                        p.Name = attr.Value;
                    }
                    else if (attr.Name == "Default")
                    {
                        p.Values = attr.Value;
                    }
                    else if (attr.Name == "Count")
                    {
                        p.Count = int.Parse(attr.Value);
                    }
                }
                p.Type = ParameterTypeName.TypeFromString(node.Name);
                if (p.Type == ParameterType.SEQUENCEPARAMETER)
                {
                    p.Type = ParameterType.SEQUENCEPARAMETER;
                    p.Params = parseParameterXmlList(node.ChildNodes, component);
                }
                else if (p.Type == ParameterType.COMPOUNDPARAMETER)
                {
                    p.Type = ParameterType.COMPOUNDPARAMETER;
                    p.Params = parseParameterXmlList(node.ChildNodes, component);
                }
                else
                {

                }
                p.ParentComponent = component;
                parameterList.Add(p);
            }
            return parameterList;
        }

        public static ObservableCollection<Parameter> parseParameterXmlList(XmlNodeList parameterData, IParameterContainer component)
        {
            return parseParameterXmlList(parameterData, component, null);
        }
    }
}
