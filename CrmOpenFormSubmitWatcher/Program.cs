using NLog;
using System;

namespace CrmOpenFormSubmitWatcher
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {

            try
            {
                logger.Info("Main({0})", args);

                // If a directory is not specified, exit program.
                if (args.Length != 1)
                {
                    // Display the proper way to call the program.
                    logger.Warn("Usage: CrmOpenFormSubmitWatcher.exe (directory)");
                    return;
                }

                string path = args[0];

                FormSubmitter submitter = new FormSubmitter(path);

                // Wait for the user to quit the program.
                Console.WriteLine("Press \'q\' to quit the sample.");
                while (Console.Read() != 'q') ;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, ex.Message);
                throw ex;
            }
            
        }

        

    }
}
