using NLog;
using System;
using System.IO;
using System.Threading;

namespace ProdwareFileWatcher
{
    class FileReader
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public FileReader()
        {
            logger.Debug("FileReader()");
        }

        public string ReadFile(string path)
        {

            logger.Debug("ReadFile({0})", path);

            string result = string.Empty;

            try
            {
                // read file without locking
                if (GetExclusiveFileLock(path))
                {
                    using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var textReader = new StreamReader(fileStream))
                        {
                            var content = textReader.ReadToEnd();
                            result = content;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Makes 1..MaximumAttemptsAllowed to get access to the file to handle file created in chunks.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
    }
}
