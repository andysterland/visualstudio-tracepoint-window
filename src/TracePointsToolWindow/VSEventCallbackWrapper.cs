using Microsoft.VisualStudio.Debugger.Interop;
using System;

namespace TracePointsToolWindow
{
    /// <summary>
    /// 
    /// </summary>
    public class VSEventCallbackWrapper : IDebugEventCallback2
    {
        public event EventHandler<OnTracePointAddedEventArgs> OnTracePointAdded;
        public event EventHandler<OnSessionStartEventArgs> OnSessionStart;

        private DateTime _sessionStartTime;

        public int Event(
           IDebugEngine2 pEngine,
           IDebugProcess2 pProcess,
           IDebugProgram2 pProgram,
           IDebugThread2 pThread,
           IDebugEvent2 pEvent,
           ref Guid riidEvent,
           uint dwAttrib
        )
        {
            if (riidEvent == typeof(IDebugProgramCreateEvent2).GUID)
            {
                RaiseStartEvent();
            }
            else if (riidEvent == typeof(IDebugMessageEvent2).GUID)
            {
                IDebugMessageEvent2 debugMessageEvent = pEvent as IDebugMessageEvent2;

                enum_MESSAGETYPE[] messageTypes = new enum_MESSAGETYPE[2];
                messageTypes[0] = enum_MESSAGETYPE.MT_OUTPUTSTRING;
                messageTypes[1] = enum_MESSAGETYPE.MT_REASON_TRACEPOINT;
                string message;
                uint dwType;
                string helpFilename;
                uint helpId;

                debugMessageEvent.GetMessage(messageTypes, out message, out dwType, out helpFilename, out helpId);

                // MessageEvents always have a \n on the end
                message = message.TrimEnd(Environment.NewLine.ToCharArray());

                RaiseDebugMessageEvent(message);
            }
            return 0;
        }

        private void RaiseStartEvent()
        {
            this._sessionStartTime = DateTime.Now;

            if (OnSessionStart != null)
            {
                this.OnSessionStart.Invoke(this, new OnSessionStartEventArgs(this._sessionStartTime));
            }
        }

        private void RaiseDebugMessageEvent(string Message)
        {
            if (OnTracePointAdded != null)
            {
                TimeSpan deltaTime = DateTime.Now - _sessionStartTime;
                TracePointMessage tp = new TracePointMessage(DateTime.Now, deltaTime.TotalMilliseconds, Message);
                OnTracePointAddedEventArgs tpEvent = new OnTracePointAddedEventArgs(tp);

                this.OnTracePointAdded.Invoke(this, tpEvent);
            }
        }
    }
    public class OnSessionStartEventArgs : EventArgs
    {
        public DateTime StartDateTime { get; set; }

        public OnSessionStartEventArgs(DateTime StartDateTime)
        {
            this.StartDateTime = StartDateTime;
        }
    }

    public class OnTracePointAddedEventArgs : EventArgs
    {
        public TracePointMessage TracePoint { get; set; }
        public OnTracePointAddedEventArgs(TracePointMessage TracePoint)
        {
            this.TracePoint = TracePoint;
        }
    }
}
