using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CogaenDataItems.DataItems;
using CogaenEditor2.Configuration;

namespace CogaenEditor2.Helper
{
    public partial class DataItemsTools
    {
        public static Element.ElementSemantic getElementSemantic(String id)
        {
            Element.ElementSemantic semantic = Element.ElementSemantic.NONE;
            #region 3deditor
            OptionGroup _3dEditor = Config.getConfig().GetOptionGroup("3D Editor Options");
            if (_3dEditor != null)
            {
                foreach (IOption option in _3dEditor.Options)
                {
                    if (option.Name == "Model Component")
                    {
                        ListOption list = option as ListOption;
                        if (list != null)
                        {
                            foreach (String val in list.Value)
                            {
                                if (val == id)
                                    semantic |= Element.ElementSemantic.MESH;
                            }
                        }
                    }
                    if (option.Name == "Position Component")
                    {
                        ListOption list = option as ListOption;
                        if (list != null)
                        {
                            foreach (String val in list.Value)
                            {
                                if (val == id)
                                    semantic |= Element.ElementSemantic.POSITION3D;
                            }
                        }
                    }

                    if (option.Name == "Orientation Component")
                    {
                        ListOption list = option as ListOption;
                        if (list != null)
                        {
                            foreach (String val in list.Value)
                            {
                                if (val == id)
                                    semantic |= Element.ElementSemantic.ORIENTATION3D;
                            }
                        }
                    }
                    if (option.Name == "Scale Component")
                    {
                        ListOption list = option as ListOption;
                        if (list != null)
                        {
                            foreach (String val in list.Value)
                            {
                                if (val == id)
                                    semantic |= Element.ElementSemantic.DIMENSION3D;
                            }
                        }
                    }
                    if (option.Name == "Camera Component")
                    {
                        ListOption list = option as ListOption;
                        if (list != null)
                        {
                            foreach (String val in list.Value)
                            {
                                if (val == id)
                                    semantic |= Element.ElementSemantic.CAMERA;
                            }
                        }
                    }
                }
            }
            #endregion

            #region state machine
            OptionGroup stateMachines = Config.getConfig().GetOptionGroup("StateMachines");
            if (stateMachines != null)
            {
                foreach (IOption option in stateMachines.Options)
                {
                    if (option.Name == "State Component")
                    {
                        ListOption list = option as ListOption;
                        if (list != null)
                        {
                            foreach (String val in list.Value)
                            {
                                if (val == id)
                                    semantic |= Element.ElementSemantic.STATE;
                            }
                        }
                    }
                    else if (option.Name == "StateMachine Component")
                    {
                        ListOption list = option as ListOption;
                        if (list != null)
                        {
                            foreach (String val in list.Value)
                            {
                                if (val == id)
                                    semantic |= Element.ElementSemantic.STATEMACHINE;
                            }
                        }
                    }
                }
            }
            #endregion
            return semantic;
        }
    }
}
