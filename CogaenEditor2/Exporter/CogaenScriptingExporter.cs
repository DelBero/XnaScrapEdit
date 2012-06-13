using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CogaenDataItems.Exporter;
using CogaenDataItems.DataItems;

namespace CogaenEditor2.Exporter
{
    public class CogaenScriptingExporter : IScriptExporter
    {

        public String ExtensionFilter
        {
            get { return "CogaenScript file (*.csl)|*.csl"; }
        }

        public String Extension
        {
            get { return ".csl"; }
        }

        public String EntryPoint()
        {
            return "void main() {\n";
        }
        public String End()
        {
            return "}\n";
        }

        public String beginRegisterMacro(String name, String scriptName)
        {
            return "registerMacro(\"" + name + "\", \""+ scriptName  +"\");";
        }
        public String endRegisterMacro()
        {
            return "\n";
        }

        public String callMacro(String macroName, KeyValuePair<String, String>[] parameters)
        {
            String s = "Macro(\"" + macroName + "\"";
            foreach (KeyValuePair<String, String> parameter in parameters)
            {
                // TODO check Syntax!
                s += " " + parameter.Key + "=\"" + parameter.Value + "\""; // could be wrong :)
            }
            s += ");\n";
            return s;
        }
        public String endCallMacro()
        {
            return "";
        }

        public String beginMacroRegistration()
        {
            return "void register() {\n";
        }
        public String endMacroRegistration()
        {
            return "}\n";
        }

        public String beginMacroBody()
        {
            return "void execute() {\n";
        }
        public String endMacroBody()
        {
            return "}\n";
        }

        

        public String registerMacroParameter(String name, ParameterType type, String defaultValues)
        {
            String param = "";
            String[] parameters = defaultValues.Split(',');
            switch (type)
            {
                case ParameterType.DOUBLE:
                {
                    param += "doubleParameter(\"";
                    break;
                }
                case ParameterType.INTEGER:
                {
                    param += "intParameter(\"";
                    break;
                }
                case ParameterType.STRING:
                {
                    param += "stringParameter(\"";
                    break;
                }
                case ParameterType.ID:
                {
                    param += "stringParameter(\"";
                    break;
                }
            }
            param += name + "\", " + parameters.Length + ", " + defaultValues + ");\n";
            return param;
        }

        public String beginFunction(String name)
        {
            return "void " + name + "() {\n";
        }

        public String functionParameter(String type, String name)
        {
            return " " + name + " ";
        }

        public String endFunction()
        {
            return "}\n";
        }

        public String beginGameObject()
        {
            return "GameObject(createId()) {\n";
        }
        public String beginGameObject(String name)
        {
            return "GameObject(\"" + name + "\") {\n";
        }
        public String endGameObject()
        {
            return "}\n";
        }

        public String beginComponent(String name)
        {
            return "Component(" + name + ") {\n";
        }
        public String endComponent()
        {
            return "}\n";
        }

        public String beginParameter(String name)
        {
            return "Parameter(\""+name+"\") {\n";
        }
        public String endParameter()
        {
            return "}\n";
        }
        public String setParameterValue(String name, String value, bool QuotationMark = false)
        {
            String quot = "";
            if (QuotationMark)
            {
                quot = "\"";
            }
            return "setParameter(\"" + name + "\", " + quot + value + quot + ");\n";
        }

        public override string ToString()
        {
            return "CogaenScriptExporter";
        }
    }
}
