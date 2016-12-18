using Easy.MessageHub;
using ProdwareFileWatcher;
using System;

namespace CrmOpenFormSubmitWatcher
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                // If a directory is not specified, exit program.
                if (args.Length != 1)
                {
                    // Display the proper way to call the program.
                    Console.WriteLine("Usage: CrmOpenFormSubmitWatcher.exe (directory)");
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
                Console.Error.WriteLine("Exception: " + ex.Message);
                throw ex;
            }
            
        }

        

    }
}
