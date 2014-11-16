using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Logging
    {
        public void Log(LogLevels level, String sender, String message, Exception exception = null)
        {
            // TODO: Get class name of caller automatically... Reflection FTW
        }
    }
}
