// Guids.cs
// MUST match guids.h
using System;

namespace CogaenEditExtension
{
    static class GuidList
    {
        public const string guidCogaenEditExtensionPkgString = "76991994-c77e-469c-94e4-7575746b341d";
        public const string guidCogaenEditExtensionCmdSetString = "30017b07-a945-45b3-82f8-73d331235878";
        public const string guidToolWindowPersistanceString = "d0548269-fd17-4a07-a272-764499acd936";
        public const string guidCogaenEditExtensionEditorFactoryString = "4108399a-f4dd-432b-a611-19470f23901d";
        public const string guidConnectionLogGuidString = "1B1A8731-1A88-4620-9299-856D2B0E6585";

        public const string guidIronPythonLanguageString = "ae8ce01a-b3ff-4c19-8c80-54669c197f2c";

        public static readonly Guid guidCogaenEditExtensionCmdSet = new Guid(guidCogaenEditExtensionCmdSetString);
        public static readonly Guid guidCogaenEditExtensionEditorFactory = new Guid(guidCogaenEditExtensionEditorFactoryString);

        public static Guid guidConnectionLogGuid = new Guid(guidConnectionLogGuidString);
    };
}