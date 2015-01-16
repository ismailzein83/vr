
namespace TABS.Components
{
    public class AdoNetAppender : log4net.Appender.AdoNetAppender
    {
        public static string GlobalConnectionString { get; protected set; }
        public override void ActivateOptions()
        {
            lock (typeof(AdoNetAppender))
            {
                if (GlobalConnectionString == null)
                {
                    if (DataConfiguration.ConnectionStringEncrypted)
                        base.ConnectionString = WebHelperLibrary.Utility.SimpleDecode(base.ConnectionString);                    
                }
                GlobalConnectionString = base.ConnectionString;
            }
            base.ActivateOptions();
        }
    }
}
