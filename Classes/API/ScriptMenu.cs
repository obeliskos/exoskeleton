﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptMenu : IDisposable
    {
        IHostWindow host;

        public ScriptMenu(IHostWindow host)
        {
            this.host = host;
        }

        public void Initialize()
        {
            host.InitializeMenuStrip();
        }

        public void Show()
        {
            host.ShowMenu();
        }

        public void Hide()
        {
            host.HideMenu();
        }

        public void AddMenu(string menuName, string emitEventName)
        {
            host.AddMenu(menuName, emitEventName);
        }

        public void AddMenuItem(string menuName, string menuItemName, string emitEventName, string shortcutKeys)
        {
            host.AddMenuItem(menuName, menuItemName, emitEventName, shortcutKeys);
        }

        public void Dispose()
        {
        }
    }
}
