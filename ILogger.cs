using System;
using System.Collections.Generic;
using System.Text;

namespace LogLogging
{
   public interface ILogger
    {

        void LogInformation(LogLevels eventLevel, string information, Exception ex = null, params object[] values);
    }
}
