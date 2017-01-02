using CrmOpenFormSubmitWatcher;
using NLog;
using System;
using System.ServiceProcess;

namespace CrmOpenFormSubmitService
{
    public partial class ParseAndSubmitService : ServiceBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        FormSubmitter submitter = null;

        public ParseAndSubmitService()
        {
            logger.Debug("ParseAndSubmitService()");
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("OnStart {args}", args);

            try
            {
                submitter = new FormSubmitter();
                submitter.Start();
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

            try
            {
                submitter.Stop();
                submitter = null;
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                //throw ex;
            }
            
        }
    }
}
