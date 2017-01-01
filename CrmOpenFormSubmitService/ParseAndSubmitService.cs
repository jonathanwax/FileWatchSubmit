using CrmOpenFormSubmitWatcher;
using NLog;
using System;
using System.Configuration;
using System.ServiceProcess;

namespace CrmOpenFormSubmitService
{
    public partial class ParseAndSubmitService : ServiceBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        FormSubmitter submitter = null;

        public ParseAndSubmitService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("OnStart {args}", args);

            //logger.Trace("Sample trace message");
            //logger.Debug("Sample debug message");
            //logger.Info("Sample informational message");
            //logger.Warn("Sample warning message");
            //logger.Error("Sample error message");
            //logger.Fatal("Sample fatal error message");

            try
            {

                string path = ConfigurationManager.AppSettings["watchFolder"];
                int interval = 0;

                // If a directory is not specified, exit program.
                if (string.IsNullOrEmpty(path))
                {
                    // Display the proper way to call the program.
                    logger.Warn("Check config for watchFolder. It cannot be missing or empty.");
                    return;
                }

                logger.Info("Watching Folder: {0}", path);

                if (!int.TryParse(ConfigurationManager.AppSettings["watchInterval"], out interval))
                {
                    logger.Error("watchInterval is not an integer. Check config file.");
                    return;
                }

                logger.Info("Watch Interval: {0} secs", interval);

                submitter = new FormSubmitter(path, interval);
                
            }
            catch (Exception ex)
            {
                logger.Error("Exception: {0}", ex.Message);
                //throw ex; - don't throw to keep service alive
            }


        }

        protected override void OnStop()
        {
            logger.Info("OnStop");
            submitter = null;
        }
    }
}
