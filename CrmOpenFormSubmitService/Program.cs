using System.ServiceProcess;

namespace CrmOpenFormSubmitService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ParseAndSubmitService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
