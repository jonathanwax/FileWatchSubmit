using Easy.MessageHub;
using Microsoft.WindowsAzure.Storage; // Namespace for Storage Client Library
using Microsoft.WindowsAzure.Storage.File; // Namespace for File storage
using NLog;
using System;
using System.Timers;

namespace ProdwareFileWatcher
{
    public class AzureFileServicePoller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IMessageHub _hub;

        private Timer timer;
        private string Interval { get; set; }
        public string StorageConnectionString { get; set; }


        public AzureFileServicePoller(string storageConnectionString, string interval)
        {
            logger.Debug("AzureFileServicePoller({0}, {1})", storageConnectionString, interval);

            this.StorageConnectionString = storageConnectionString;
            this.Interval = interval;

            _hub = MessageHub.Instance;

        }

        ~AzureFileServicePoller()
        {
            logger.Debug("~AzureFileServicePoller() - destructor");
            StopTimer();
        }

        public void StartTimer()
        {
            logger.Debug("StartTimer()");
            try
            {
                var miliInterval = int.Parse(this.Interval) * 1000;

                logger.Info("StartTimer({0})", miliInterval);

                timer = new Timer(miliInterval);
                timer.AutoReset = true;
                timer.Elapsed += new ElapsedEventHandler(CheckFolder);
                timer.Enabled = true;

                timer.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                //throw ex;
            }


        }

        public void StopTimer()
        {
            logger.Debug("StopTimer()");
            try
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                //throw ex;
            }
        }

        private void CheckFolder(object sender, ElapsedEventArgs e)
        {
            logger.Info("CheckFolder()");

            try
            {

                // Parse the connection string and return a reference to the storage account.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

                // Create a CloudFileClient object for credentialed access to File storage.
                CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

                // Get a reference to the file share we created previously.
                CloudFileShare share = fileClient.GetShareReference("forms");

                // Ensure that the share exists.
                if (share.Exists())
                {
                    // Get a reference to the root directory for the share.
                    CloudFileDirectory rootDir = share.GetRootDirectoryReference();

                    // Get a reference to the directory we created previously.
                    CloudFileDirectory sourceDir = rootDir.GetDirectoryReference("new");
                    CloudFileDirectory targetDir = rootDir.GetDirectoryReference("done");

                    // Ensure that the directory exists.
                    if (sourceDir.Exists() && targetDir.Exists())
                    {
                        
                        foreach (var fileItem in sourceDir.ListFilesAndDirectories())
                        {

                            var filename = ((CloudFile)fileItem).Name;

                            // skip files not ending with *.txt
                            if(!filename.EndsWith(".txt")) { continue; }

                            logger.Info("File: {0}", filename);

                            // Get a reference to the file we created previously.
                            CloudFile file = sourceDir.GetFileReference(filename);

                            // Ensure that the file exists.
                            if (file.Exists())
                            {
                                // Write the contents of the file to the console window.
                                var content = file.DownloadTextAsync().Result;
                                logger.Debug("Parsed: {0}", filename);

                                // Raise event to FormSubmitter to Submit Content to CRM
                                _hub.Publish<string>(content);

                                // Copy sourceDir/file.txt to targetDir/file.done, and delete sourceDir/file.txt
                                var destinationFilename = filename; //.Replace(".txt", ".done");
                                CloudFile destinationFile = targetDir.GetFileReference(destinationFilename);
                                var result = destinationFile.StartCopyAsync(file).Result;
                                logger.Debug("Copied: {0} to {1}/{2}", filename, targetDir.Name, destinationFilename);

                                // Delete file in sourceDir (so it is NOT processed again)
                                file.Delete();
                                logger.Debug("Deleted: {0}/{1}", sourceDir.Name, filename);

                                logger.Info("Done: {0}", filename);

                            }
                        }
                    }
                    else
                    {
                        logger.Error("Missing Directories on Cloud: {0}, {1}", "new", "done");
                    }
                    
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }

        }
        
    }
}
