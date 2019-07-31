namespace TracePointsToolWindow
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using System.ComponentModel.Design;
    using System.Windows.Documents;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("20158173-f838-4bd3-aba8-cf8859207839")]
    public class TracePointHostWindow1 : ToolWindowPane
    {
        public const string TracePointToolWindowPackageGuid = "{1b42a975-241e-4e16-ac65-4bc607477497}";  // get the GUID from the .vsct file
        public const int TracePointToolbar = 0x1000;

        private Random _randomGenerator = new Random();
        private TracePointHostWindow1Control _ucTracePoint;
        private VSEventCallbackWrapper _debugEventCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="TracePointHostWindow1"/> class.
        /// </summary>
        public TracePointHostWindow1() : base(null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.Caption = AppStringResources.lblToolWindowCaption;
            this._ucTracePoint = new TracePointHostWindow1Control();
            this._debugEventCallback = new VSEventCallbackWrapper();
            this._debugEventCallback.OnSessionStart += _debugEventCallback_OnSessionStart;
            this._debugEventCallback.OnTracePointAdded += _debugEventCallback_OnTracePointAdded;
            
            IVsDebugger debugService = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsShellDebugger)) as IVsDebugger;
            if (debugService != null)
            {
                // Register for debug events.
                // Assumes the current class implements IDebugEventCallback2.
                debugService.AdviseDebugEventCallback(this._debugEventCallback);
            }

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            
            this.Content = _ucTracePoint;
        }

        private void _debugEventCallback_OnTracePointAdded(object sender, OnTracePointAddedEventArgs e)
        {
            this._ucTracePoint.AddTracePoint(e.TracePoint);
        }

        private void _debugEventCallback_OnSessionStart(object sender, OnSessionStartEventArgs e)
        {
            if(Config.ClearOnSessionStart)
            {
                this._ucTracePoint.ClearAllTracePoints();
            }
        }
    }
}
