using System;
using Vanrise.Common;

namespace TOne.WhS.DBSync.Business
{
    public class DBSyncLogger
    {

        public void WriteException(Exception ex)
        {
            LoggerFactory.GetExceptionLogger().WriteException(ex);
        }

        public void WriteInformation(string message)
        {
            LoggerFactory.GetLogger().WriteInformation(message);
        }

        public void WriteEntry(Vanrise.Entities.LogEntryType logEntryType, string message)
        {
            LoggerFactory.GetLogger().WriteEntry(logEntryType, message);
        }

        public void WriteVerbose(string message)
        {
            LoggerFactory.GetLogger().WriteVerbose(message);
        }

        public void WriteError(string message)
        {
            LoggerFactory.GetLogger().WriteError(message);
        }

        public void WriteWarning(string message)
        {
            LoggerFactory.GetLogger().WriteWarning(message);
        }
    }
}
