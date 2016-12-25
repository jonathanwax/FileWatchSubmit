using Easy.MessageHub;
using NLog;
using ProdwareFileWatcher;
using ProdwareSoapClient;
using System;

namespace CrmOpenFormSubmitWatcher
{
    class FormSubmitter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IMessageHub _hub;
        private readonly Guid _subscriptionToken;

        public FormSubmitter(string path)
        {
            logger.Info("FormSubmitter({path})", path);
            // subscribe to events
            _hub = MessageHub.Instance;
            _subscriptionToken = _hub.Subscribe<string>(OnFileCreated);

            FileWatcher watcher = new FileWatcher(path);

        }

        ~FormSubmitter()
        {
            _hub.UnSubscribe(_subscriptionToken);
            _hub.ClearSubscriptions();
        }

        private void OnFileCreated(string content)
        {
            logger.Info("OnFileCreated({0})", content);
            string xml = content;
            Request request = new Request();
            int result = request.Submit(xml);

            if(result == 200)
            {
                logger.Info("Done");
            }
            else
            {
                logger.Error("ERROR {0}", result);
            }
        }

    }
}
