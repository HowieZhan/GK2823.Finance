

using NLog;

namespace GK2823.UtilLib.Helpers
{
    public class NLogHelper
    {
        public static ILogger GetLogger()
        {
            if (_logger == null)
            {
                InitLogger();
            }
            return _logger;
        }

        static ILogger _logger;
        private static void InitLogger()
        {
            _logger = LogManager.GetLogger("");
        }
    }
}
