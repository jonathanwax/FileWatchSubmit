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
                logger.Debug("Main({0})", args);

                FormSubmitter submitter = new FormSubmitter();

                // Wait for the user to quit the program.
                Console.WriteLine("Press \'q\' to quit the sample.");
                while (Console.Read() != 'q') ;
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                throw ex;
            }
            
        }

        

    }
}
