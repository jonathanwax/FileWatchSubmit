using Easy.MessageHub;
using NLog;
using System;
using System.IO;
using System.Security.Permissions;
using System.Threading;

namespace ProdwareFileWatcher
{
    public class FileWatcher
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IMessageHub _hub;

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public FileWatcher(string path, string domain, string user, string password)
        {
            try
            {

                logger.Info("FileWatcher({0})", path);

                _hub = MessageHub.Instance;

                // Create a new FileSystemWatcher and set its properties.
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = path;
               
                /* Watch for changes in LastAccess and LastWrite times, and
                   the renaming of files or directories. */
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                // Only watch text files.
                watcher.Filter = "*.txt";

                // Add event handlers.
                //watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Created += new FileSystemEventHandler(OnCreated);
                //watcher.Deleted += new FileSystemEventHandler(OnChanged);
                //watcher.Renamed += new RenamedEventHandler(OnRenamed);

                // Begin watching.
                watcher.EnableRaisingEvents = true;


            }
            catch (Exception ex)
            {
                logger.Fatal(ex, ex.Message);
                throw ex;
            }
        }

        // Define the event handlers.
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            logger.Info("OnCreated({0}, {1})", e.FullPath, e.ChangeType);

            try
            {
                
                // read file without locking
                if (GetExclusiveFileLock(e.FullPath))
                {
                    using (var fileStream = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var textReader = new StreamReader(fileStream))
                        {
                            var content = textReader.ReadToEnd();
                            _hub.Publish<string>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Fatal(ex, ex.Message);
                throw ex;
            }

        }

        private bool GetExclusiveFileLock(string path)
        {
            var fileReady = false;
            const int MaximumAttemptsAllowed = 30;
            var attemptsMade = 0;

            while (!fileReady && attemptsMade <= MaximumAttemptsAllowed)
            {
                try
                {
                    using (File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        fileReady = true;
                    }
                }
                catch (IOException)
                {
                    attemptsMade++;
                    Thread.Sleep(100);
                }
            }

            return fileReady;
        }

        //private static void OnRenamed(object source, RenamedEventArgs e)
        //{
        //    // Specify what is done when a file is renamed.
        //    Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        //}
    }
}
