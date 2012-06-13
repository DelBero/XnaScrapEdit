using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using CogaenDataItems.DataItems;

namespace CogaenEditorControls.Resources.TemplateSelectors
{
    public class MessageParameterTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is Parameter)
            {
                Parameter parameter = item as Parameter;
                if (parameter.Type == ParameterType.DOUBLE)
                {
                    return element.FindResource("MessageFloatParameterTemplate") as DataTemplate;
                }
                else if (parameter.Type == ParameterType.ID || parameter.Type == ParameterType.STRING)
                {
                    if (parameter.Semantic == ParameterSemantic.MESH || parameter.Semantic == ParameterSemantic.TEXTURE || parameter.Semantic == ParameterSemantic.MATERIAL)
                    {
                        return element.FindResource("MessageComboBoxParameterTemplate") as DataTemplate;
                    }
                    else
                    {
                        DataTemplate temp = element.FindResource("MessageStringParameterTemplate") as DataTemplate;
                        return temp;
                    }
                }
                else if (parameter.Type == ParameterType.BOOL)
                {
                    DataTemplate temp = element.FindResource("MessageBoolParameterTemplate") as DataTemplate;
                    return temp;
                }
                else
                {
                    return element.FindResource("MessageStringParameterTemplate") as DataTemplate;
                }
            }
            return null;
        }

    }
}
