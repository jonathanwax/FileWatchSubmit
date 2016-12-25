using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CrmOpenFormSubmitService
{
    public partial class ParseAndSubmitService : ServiceBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public ParseAndSubmitService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("OnStart {args}", args);

            //logger.Trace("Sample trace message");
            //logger.Debug("Sample debug message");
            //logger.Info("Sample informational message");
            //logger.Warn("Sample warning message");
            //logger.Error("Sample error message");
            //logger.Fatal("Sample fatal error message");

        }

        protected override void OnStop()
        {
            logger.Info("OnStop");
        }
    }
}
