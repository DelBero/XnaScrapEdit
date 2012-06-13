using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CogaenDataItems.DataItems;
using System.Xml;
//using CogaenEditor2.Configuration;

namespace CogaenEditExtension.Helper
{
    public partial class DataItemsTools
    {
        public static ParameterSemantic getParameterSemantic(XmlNode parameter, Element element, Parameter param)
        {
            //App app = App.Current as App;
            //String parameterName = "";
            //int size = 1;
            //foreach (XmlAttribute attr in parameter.Attributes)
            //{
            //    if (attr.Name == "Name")
            //    {
            //        parameterName = attr.Value;
            //    }
            //    if (attr.Name == "Default")
            //    {
            //        size = attr.Value.Split(',').Length;
            //    }
            //}
            //OptionGroup optionGroup = null;
            //if (element.Semantic > Element.ElementSemantic.NONE && element.Semantic <= Element.ElementSemantic.CAMERA)
            //{
            //    optionGroup = (app.Config.GetOptionGroup("3D Editor Options") as OptionGroup);
            //}
            //else if (element.Semantic >= Element.ElementSemantic.STATE && element.Semantic <= Element.ElementSemantic.STATEMACHINE)
            //{
            //    optionGroup = (app.Config.GetOptionGroup("StateMachines") as OptionGroup);
            //}

            //if (optionGroup != null)
            //{
            //    foreach (IOption option in optionGroup.Options)
            //    {
            //        if (option.Type == StringSemanticOption.m_type)
            //        {
            //            StringSemanticOption Param = option as StringSemanticOption;
            //            if (Param != null)
            //            {
            //                String[] values = Param.Value.Split(',');
            //                foreach (String value in values)
            //                {
            //                    if (value == parameterName)
            //                    {
            //                        if (Param.Semantic == ParameterSemantic.MESH)
            //                        {
            //                            ResourceFolder folder = app.Data.Resources.getFolder("Meshes");
            //                            if (folder != null)
            //                            {
            //                                param.SemanticValues = folder.Resources;
            //                            }
            //                        }
            //                        else if (Param.Semantic == ParameterSemantic.MATERIAL)
            //                        {
            //                            ResourceFolder folder = app.Data.Resources.getFolder("Materials");
            //                            if (folder != null)
            //                            {
            //                                param.SemanticValues = folder.Resources;
            //                            }
            //                        }
            //                        else if (Param.Semantic == ParameterSemantic.TEXTURE)
            //                        {
            //                            ResourceFolder folder = app.Data.Resources.getFolder("Textures");
            //                            if (folder != null)
            //                            {
            //                                param.SemanticValues = folder.Resources;
            //                            }
            //                        }
            //                        return Param.Semantic;
            //                    }
            //                    //Parameter parameter = findParameter(component, dimension, value.Trim());
            //                }
            //            }
            //        }
            //    }
            //}
            return ParameterSemantic.NONE;
        }
        public static ParameterSemantic getParameterSemantic(Element element, Parameter param)
        {
            //App app = App.Current as App;
            //OptionGroup optionGroup = null;
            //Config config = app.Config;
            //if (element.Semantic > Element.ElementSemantic.NONE && element.Semantic <= Element.ElementSemantic.CAMERA)
            //{
            //    optionGroup = (config.GetOptionGroup("3D Editor Options") as OptionGroup);
            //}
            //else if (element.Semantic >= Element.ElementSemantic.STATE && element.Semantic <= Element.ElementSemantic.STATEMACHINE)
            //{
            //    optionGroup = (config.GetOptionGroup("StateMachines") as OptionGroup);
            //}

            //if (optionGroup != null)
            //{
            //    foreach (IOption option in optionGroup.Options)
            //    {
            //        if (option.Type == StringSemanticOption.m_type)
            //        {
            //            StringSemanticOption Param = option as StringSemanticOption;
            //            if (Param != null)
            //            {
            //                String[] values = Param.Value.Split(',');
            //                foreach (String value in values)
            //                {
            //                    if (value == param.Name)
            //                    {
            //                        if (Param.Semantic == ParameterSemantic.MESH)
            //                        {
            //                            ResourceFolder folder = app.Data.Resources.getFolder("Meshes");
            //                            if (folder != null)
            //                            {
            //                                param.SemanticValues = folder.Resources;
            //                            }
            //                        }
            //                        else if (Param.Semantic == ParameterSemantic.MATERIAL)
            //                        {
            //                            ResourceFolder folder = app.Data.Resources.getFolder("Materials");
            //                            if (folder != null)
            //                            {
            //                                param.SemanticValues = folder.Resources;
            //                            }
            //                        }
            //                        else if (Param.Semantic == ParameterSemantic.TEXTURE)
            //                        {
            //                            ResourceFolder folder = app.Data.Resources.getFolder("Textures");
            //                            if (folder != null)
            //                            {
            //                                param.SemanticValues = folder.Resources;
            //                            }
            //                        }
            //                        return Param.Semantic;
            //                    }
            //                    //Parameter parameter = findParameter(component, dimension, value.Trim());
            //                }
            //            }
            //        }
            //    }
            //}
            return ParameterSemantic.NONE;
        }
    }
}
