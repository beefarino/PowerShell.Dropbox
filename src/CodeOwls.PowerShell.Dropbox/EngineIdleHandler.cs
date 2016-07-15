using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace CodeOwls.PowerShell.Dropbox
{
    public static class EngineIdleManager
    {
        public static event EventHandler OnEngineIdle;
        static bool IsRegistered = false;

        public static object CurrentEventJob { get; private set; } = null;

        public static void RegisterForNextEngineIdle(SessionState sessionState )
        {
            if (IsRegistered)
            {
                return;
            }

            IsRegistered = true;
            try
            {
                var results = sessionState.InvokeCommand.InvokeScript(RegistrationScript);
                CurrentEventJob = results.FirstOrDefault()?.BaseObject;
            }
            catch
            {
                IsRegistered = false;
                throw;
            }

        }
        
        const string RegistrationScript =
            @"register-engineEvent -sourceIdentifier PowerShell.OnIdle -action { 
                try {
                    [CodeOwls.PowerShell.Dropbox.EngineIdleManager]::NotifyEngineIdle( $host.runspace.sessionstateproxy ); 
                }
                finally {  
                    unregister-event -sourceIdentifier ([CodeOwls.PowerShell.Dropbox.EngineIdleManager]::CurrentEventJob.Name);
                }
            }";
        
        public static void NotifyEngineIdle(SessionStateProxy sessionState)
        {
            OnEngineIdle?.Invoke(null, EventArgs.Empty);
        }
    }
}