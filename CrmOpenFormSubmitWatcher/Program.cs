using NLog;
using System;
using System.Configuration;

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

                string path = ConfigurationManager.AppSettings["watchFolder"];
                int interval = 0;

                if (!int.TryParse(ConfigurationManager.AppSettings["watchInterval"], out interval))
                {
                    logger.Error("watchInterval is not an integer. Check config file.");
                    return;
                }

                FormSubmitter submitter = new FormSubmitter(path, interval);

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
