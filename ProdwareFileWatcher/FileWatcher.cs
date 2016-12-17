﻿using Easy.MessageHub;
using System;
using System.IO;
using System.Security.Permissions;

namespace ProdwareFileWatcher
{
    public class FileWatcher
    {
        private readonly IMessageHub _hub;

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public FileWatcher(string path)
        {
            try
            {

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
                throw ex;
            }
        }

        // Define the event handlers.
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);

            try
            {
                string text = File.ReadAllText(e.FullPath);
                _hub.Publish<string>(text);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //private static void OnRenamed(object source, RenamedEventArgs e)
        //{
        //    // Specify what is done when a file is renamed.
        //    Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        //}
    }
}
