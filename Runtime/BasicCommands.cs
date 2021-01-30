namespace Exodia
{
    ///<summary>
    ///This script houses all the basic commands that the DevConsole uses.
    ///<para>- These commands are used in all of Exodia's games.</para>
    ///<para>- That's why it's smart that we use the same script for all of the basic commands so we are save little time from the copy pasting.</para>
    ///</summary>
    public static class BasicCommands
    {
    // DEV CONSOLE
        ///<summary>Changes the DevConsole's logging option</summary>
        public class Logger : Command {
            public override string[] Names() { return new string[] { "log", }; }
            public override string Function(params object[] input) {
                if (input.Length == 1) {
                    eConsoleLog result;
                    if (System.Enum.TryParse<eConsoleLog>((string)input[0], true, out result)) {
                        DevConsole.instance.consoleLog = result;
                        return "changed the console's logging to " + result.ToString();
                    }
                }
                return "error: incorrect console logging enum";
            }
        }

        ///<summary>Debug.Logs the input.</summary>
        public class Log : Command {
            public override string[] Names() { return new string[] { "debug", "echo", }; }
            public override string Function(params object[] input) {
                string output = "";
                for (int i = 0; i < input.Length; i++) {
                    if (i > 0) output += " ";
                    output += (string)input[i];
                }
                return output;
            }
        }
        
        ///<summary>Clears the console completely from all logs.</summary>
        public class Clear : Command {
            public override string[] Names() { return new string[] { "clear", "cls", }; }
            public override string Function(params object[] input) {
                DevConsole.instance.Clear();
                return "";
            }
        }

        ///<summary>Returns the console window back to the starting position.</summary>
        public class Home : Command {
            public override string[] Names() { return new string[] { "home", }; }
            public override string Function(params object[] input) {
                DevConsole.instance.Home();
                return "";
            }
        }

        ///<summary>Hides the console.</summary>
        public class Hide : Command {
            public override string[] Names() { return new string[] { "hide", "exit", }; }
            public override string Function(params object[] input) {
                DevConsole.instance.Toggle();
                return "";
            }
        }
    }
}