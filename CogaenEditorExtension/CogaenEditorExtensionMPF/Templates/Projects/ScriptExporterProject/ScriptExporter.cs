using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CogaenDataItems.Exporter;
using CogaenDataItems.DataItems;

namespace XnaScriptExporter
{
    public class XnaXmlScriptExporter : IScriptExporter
    {
        public String ExtensionFilter
        {
            get { return "XmlScript file (*.xmlScript)|*.xmlScript"; }
        }

        public String Extension
        {
            get { return ".xmlScript"; }
        }

        Stack<String> m_functionStack = new Stack<String>();

        public String EntryPoint()
        {
            return "<Root><Main>\n";
        }
        public String End()
        {
            return "</Main></Root>";
        }

        public String beginRegisterMacro(String name, String scriptName)
        {
            return "<RegisterMacro Id=\"" + name + "\" Script=\"" + scriptName + "\"";
        }
        public String endRegisterMacro()
        {
            return ">\n";
        }

        public String callMacro(String macroName, KeyValuePair<String, String>[] parameters)
        {
            String s = "<Macro";
            foreach (KeyValuePair<String, String> parameter in parameters)
            {
                s += " " + parameter.Key + "=\"" + parameter.Value + "\"";
            }
            s += "/>\n";
            return s;
        }

        public String endCallMacro()
        {
            return "";
        }

        public String beginMacroRegistration()
        {
            return "<Root><Main";
        }
        public String endMacroRegistration()
        {
            return ">";
        }

        public String beginMacroBody()
        {
            return "";
        }

        public String endMacroBody()
        {
            return "</Main></Root>";
        }

        public String registerMacroParameter(String name, ParameterType type, String defaultValues)
        {
            String s = " " + name + "=\"" + defaultValues + "\"";
            return s;
        }

        public String beginFunction(String name)
        {
            m_functionStack.Push(name);
            return "<" + name + ">\n";
        }
        public String functionParameter(String type, String name)
        {
            return "";
        }
        public String endFunction()
        {
            String name = m_functionStack.Pop();
            return "</" + name + ">\n";
        }

        public String beginGameObject()
        {
            return "<CreateId/><GameObject>\n";
        }
        public String beginGameObject(String name)
        {
            return "<GameObject Id=\"" + name + "\">\n";
        }
        public String endGameObject()
        {
            return "</GameObject>\n";
        }

        public String beginComponent(String name)
        {
            return "<Element Type=\"" + name + "\">\n";
        }
        public String endComponent()
        {
            return "</Element>\n";
        }

        public String beginParameter(String name)
        {
            return "<Parameter Name=\"" + name + "\">\n";
        }
        public String endParameter()
        {
            return "</Parameter>\n";
        }
        public String setParameterValue(String name, String value, bool QuotationMark = false)
        {
            if (value.Length > 0)
            {
                return "<Parameter Name=\"" + name + "\">\n<Value>" + value + "</Value>\n</Parameter>\n";
            }
            else
            {
                return "";
            }
        }

        public override string ToString()
        {
            return "XnaXmlScriptExporter";
        }
    }
}
