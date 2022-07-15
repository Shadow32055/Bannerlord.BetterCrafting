using BetterCrafting.Settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace BetterCrafting.Utils {
    public class Helper {
        public static string modName = "ForgotToSet";
        public static ISettings? settings;

        public static void SetModName(string name) {
            modName = name;
            DisplayFriendlyMsg(modName + " Loaded.");
            //validate();
        }

        public static void DisplayFriendlyMsg(string msg) {
            InformationManager.DisplayMessage(new InformationMessage(msg, Colors.Green));
            WriteToLog(msg);
        }

        public static void DisplayMsg(string msg) {
            InformationManager.DisplayMessage(new InformationMessage(msg));
            WriteToLog(msg);
        }

        public static void DisplayWarningMsg(string msg) {
            InformationManager.DisplayMessage(new InformationMessage(msg, Colors.Red));
            WriteToLog(msg);
        }

        public static void WriteToLog(string text) {
            Debug.Print(modName + ": " + text);
        }
    }
}