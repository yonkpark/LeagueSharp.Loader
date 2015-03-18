﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using log4net;
using LeagueSharp.Loader.Core;
using LeagueSharp.Loader.Model.Log;
using LeagueSharp.Loader.Model.Settings;

namespace LeagueSharp.Loader
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Mutex _mutex;

        static App()
        {
            DispatcherHelper.Initialize();
            Config.Initialize();
            Directories.Initialize();
            Logs.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            #region Mutex

            Log.Info(Assembly.GetExecutingAssembly().GetName().Name + " started");

            var mutexCreated = false;
            var mutexName =
                Utility.Md5Hash(Directories.LoaderFilePath + Environment.UserDomainName + Environment.UserName);
            _mutex = new Mutex(true, mutexName, out mutexCreated);

            if (!mutexCreated)
            {
                if (e.Args.Length > 0)
                {
                    var wnd = Interop.FindWindow(IntPtr.Zero, "LeagueSharp");
                    if (wnd != IntPtr.Zero)
                    {
                        Clipboard.SetText(e.Args[0]);
                        Interop.ShowWindow(wnd, 5);
                        Interop.SetForegroundWindow(wnd);
                    }
                }
                _mutex = null;
                Environment.Exit(1);
            }
            else
            {
                Log.Info("Mutex Created " + mutexName);
            }

            #endregion

            // HACK: testing
            Config.Instance.Username = "h3h3";
            foreach (
                var assembly in
                    Config.Instance.SelectedProfile.InstalledAssemblies.Where(a => a.Author == "LeagueSharp"))
            {
                GitUpdater.Clone(assembly.Location, assembly.PathToRepository, false);
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            #region Config

            if (e.ApplicationExitCode == 0)
            {
                Config.Save();
            }

            #endregion

            #region Mutex

            if (_mutex != null)
            {
                Log.Info("Release Mutex");
                _mutex.ReleaseMutex();
            }

            #endregion

            Log.InfoFormat("Exit({0})", e.ApplicationExitCode);

            base.OnExit(e);
        }
    }
}