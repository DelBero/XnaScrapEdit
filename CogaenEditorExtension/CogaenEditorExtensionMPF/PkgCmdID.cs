// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace CogaenEditExtension
{
    static class PkgCmdIDList
    {
        public const uint cmdidConnect =        0x100;
        public const uint cmdidDisconnect =     0x101;
        public const uint cmdidCogaenEdit =     0x102;
        public const uint cmdidAddContentRef =  0x103;
        public const uint cmdidConvert2Macro =  0x108;
        public const uint cmdidConvert2Script = 0x109;
        public const uint cmdidRunScript =      0x110;
        public const uint cmdidMessageWindow =  0x111;
        public const uint cmdidLive2DWindow =   0x112;
        public const uint cmdidLive3DWindow =   0x113;

        // Menus
        public const int ID_COGAEN_EDT_TLB = 0x0001;			// toolbar
        public const int IDMX_RTF = 0x0002;			// context menu
        public const int IDM_RTFMNU_ALIGN = 0x0004;
        public const int IDM_RTFMNU_SIZE = 0x0005;

        // Menu Groups
        public const int IDG_RTF_FMT_FONT1 = 0x1000;
        public const int IDG_RTF_FMT_FONT2 = 0x1001;
        public const int IDG_RTF_FMT_INDENT = 0x1002;
        public const int IDG_RTF_FMT_BULLET = 0x1003;

        public const int IDG_RTF_TLB_FONT1 = 0x1004;
        public const int IDG_RTF_TLB_FONT2 = 0x1005;
        public const int IDG_RTF_TLB_INDENT = 0x1006;
        public const int IDG_RTF_TLB_BULLET = 0x1007;
        public const int IDG_RTF_TLB_FONT_COMBOS = 0x1008;

        public const int IDG_RTF_CTX_EDIT = 0x1009;
        public const int IDG_RTF_CTX_PROPS = 0x100a;

        public const int IDG_RTF_EDITOR_CMDS = 0x100b;

        // Command IDs

        public const int icmdStrike = 0x0004;

    };
}