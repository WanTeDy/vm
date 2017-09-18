using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace VirtualMachinesForm.Helpers
{
    public static class TraceHelper
    {
        public static void TraceException(Exception e)
        {
            if (e == null)
            {
                Trace.TraceError("Exception is null");
            }
            else
            {
                do
                {
                    Trace.TraceError(e.Message);
                    Trace.TraceError(e.StackTrace);
                } while ((e = e.InnerException) != null);
            }
            Trace.TraceInformation("------------------------------------------------");
            Trace.Flush();
        }
    }
}
