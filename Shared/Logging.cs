using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Logging
    {
        public static Dictionary<LogLevels, ConsoleColor> colors;
        public void Log(LogLevels level, String message, Exception exception = null)
        {
            if (colors == null)
            {
                colors = new Dictionary<LogLevels, ConsoleColor>();
                colors.Add(LogLevels.Critical, ConsoleColor.Red);
                colors.Add(LogLevels.Error, ConsoleColor.Red);
                colors.Add(LogLevels.Warning, ConsoleColor.DarkRed);
                colors.Add(LogLevels.Debug, ConsoleColor.DarkGray);
                colors.Add(LogLevels.Informative, ConsoleColor.Gray);
                colors.Add(LogLevels.Notice, ConsoleColor.Green);
                colors.Add(LogLevels.SuccessAudit, ConsoleColor.Green);
            }

            System.Console.ForegroundColor = colors[level];

            // TODO: Get class name of caller automatically... Reflection FTW
            System.Console.WriteLine(DateTime.Now.ToString("s") + "  " + message);

            if (exception != null)
                System.Console.WriteLine(exception.ToString());
        }
    }
}
