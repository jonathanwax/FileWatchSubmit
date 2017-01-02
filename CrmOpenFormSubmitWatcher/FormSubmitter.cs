using Easy.MessageHub;
using NLog;
using ProdwareFileWatcher;
using ProdwareSoapClient;
using System;
using System.Configuration;

namespace CrmOpenFormSubmitWatcher
{
    class FormSubmitter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IMessageHub _hub;
        private Guid _subscriptionToken;
        private AzureFileServicePoller monitor;

        public string CRMOpenUrl { get; set; }
        public string CRMSoapAction { get; set; }
        public string WatchInterval { get; set; }
        public string StorageConnectionString { get; set; }

        public FormSubmitter()
        {
            logger.Debug("FormSubmitter()");
        }

        ~FormSubmitter()
        {
            logger.Debug("~FormSubmitter()");
            Stop();
        }

        public void Start()
        {

            try
            {
                logger.Info("Start()");

                // get config info
                this.CRMOpenUrl = ConfigurationManager.AppSettings["CRMOpenUrl"];
                this.CRMSoapAction = ConfigurationManager.AppSettings["CRMSoapAction"];
                this.WatchInterval = ConfigurationManager.AppSettings["WatchInterval"];
                this.StorageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

                if (string.IsNullOrEmpty(this.CRMOpenUrl)
                    || string.IsNullOrEmpty(this.CRMSoapAction)
                    || string.IsNullOrEmpty(this.WatchInterval)
                    || string.IsNullOrEmpty(StorageConnectionString))
                {
                    logger.Error("Configuration file is missing: CRMOpenUrl, CRMSoapAction, WatchInterval, StorageConnectionString. ABORTING");
                    return;
                }

                logger.Info("WatchInterval = {0}", this.WatchInterval);

                // subscribe to events
                _hub = MessageHub.Instance;
                _subscriptionToken = _hub.Subscribe<string>(OnFileCreated);

                monitor = new AzureFileServicePoller(this.StorageConnectionString, this.WatchInterval);

                monitor.StartTimer();
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                //throw;
            }
            
        }

        public void Stop()
        {
            try
            {
                logger.Info("Stop()");

                monitor.StopTimer();
                monitor = null;

                _hub.UnSubscribe(_subscriptionToken);
                _hub.ClearSubscriptions();
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                //throw ex;
            }
        }

        private void OnFileCreated(string content)
        {
            try
            {
                logger.Debug("OnFileCreated({0})", content);

                string xml = content;
                Request request = new Request();
                int result = request.Submit(this.CRMOpenUrl, this.CRMSoapAction, xml);

                if (result == 200)
                {
                    logger.Info("Submitted");
                }
                else
                {
                    logger.Error("ERROR {0}", result);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                //throw ex;
            }
        }

    }
}
