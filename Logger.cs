namespace PlaywrightTests
{
    public class Logger
    {
        public void GlobalSetup()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        public static readonly ILog log = LogManager.GetLogger(typeof(Logger));
    }
}
