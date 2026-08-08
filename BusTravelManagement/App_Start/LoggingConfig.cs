using log4net;
using log4net.Config;
using System.IO;
using System.Web;

namespace BusTravelManagement.App_Start
{
    public class LoggingConfig
    {
        public static void Configure()
        {
            var logConfigPath = HttpContext.Current?.Server?.MapPath("~/Web.config");
            if (File.Exists(logConfigPath))
            {
                XmlConfigurator.Configure(new FileInfo(logConfigPath));
            }

            var logger = LogManager.GetLogger(typeof(LoggingConfig));
            logger.Info("Logging system initialized successfully.");
        }
    }
}
