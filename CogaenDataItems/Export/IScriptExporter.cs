using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CogaenDataItems.DataItems;

namespace CogaenDataItems.Exporter
{
    public interface IScriptExporter
    {
        String ExtensionFilter
        {
            get;
        }

        String Extension
        {
            get;
        }


        String EntryPoint();
        String End();

        String beginRegisterMacro(String name, String scriptName);
        String endRegisterMacro();

        String callMacro(String macroName, KeyValuePair<String, String>[] parameters);
        String endCallMacro();

        String beginMacroRegistration();
        String endMacroRegistration();

        String beginMacroBody();
        String endMacroBody();

        String registerMacroParameter(String name, ParameterType type, String defaultValues);

        String beginFunction(String name);
        String functionParameter(String type, String name);
        String endFunction();

        String beginGameObject(); // uses createId as name
        String beginGameObject(String name);
        String endGameObject();

        String beginComponent(String name);
        String endComponent();

        String beginParameter(String name);
        String endParameter();
        String setParameterValue(String name, String value, bool QuotationMark = false);
    }
}
