using Easy.MessageHub;
using ProdwareFileWatcher;
using ProdwareSoapClient;
using System;

namespace CrmOpenFormSubmitWatcher
{
    class FormSubmitter
    {
        private readonly IMessageHub _hub;
        private readonly Guid _subscriptionToken;

        public FormSubmitter(string path)
        {
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
            string xml = content;
            Request request = new ProdwareSoapClient.Request();
            request.Submit(xml);
        }

    }
}
