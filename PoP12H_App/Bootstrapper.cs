using Caliburn.Micro;
using Newtonsoft.Json.Linq;
using PoP12H_App.Models;
using PoP12H_App.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows;

namespace PoP12H_App
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
