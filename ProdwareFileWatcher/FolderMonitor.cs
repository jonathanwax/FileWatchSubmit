using Easy.MessageHub;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ProdwareFileWatcher
{
    /// <summary>
    /// Monitor a Folder/Directory based on FolderMonitorIntervalSec
    /// </summary>
    public class FolderMonitor
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IMessageHub _hub;

        Timer timer;
        public string FolderPath { get; set; }
        private int Interval { get; set; }
        public DateTime LastChecked { get; set; }

        public FolderMonitor(string path, int interval)
        {
            logger.Debug("FolderMonitor({0}, {1})", path, interval);

            this.FolderPath = path;
            this.Interval = interval * 1000;
        }

        ~FolderMonitor()
        {
            logger.Debug("~FolderMonitor() - destructor");
            StopTimer();
        }

        public void StartTimer()
        {
            logger.Debug("StartTimer()");
            try
            {
                logger.Debug("StartTimer({0})", this.Interval);
                timer = new Timer(this.Interval);
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
            logger.Debug("CheckFolder()");

            try
            {
                LastChecked = DateTime.Now;

                string[] files = System.IO.Directory.GetFiles(this.FolderPath, "*.txt", System.IO.SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    logger.Debug("file: {0}", file);

                    var reader = new FileReader();
                    var content = reader.ReadFile(file);

                    logger.Debug("content = {0}", content);

                    // change file extension so it is not read again
                    Path.ChangeExtension(file, ".done");

                    logger.Debug("file - DONE");

                    _hub.Publish<string>(content);
                }

                timer.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
            
        }

        


    }
}
