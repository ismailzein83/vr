using System;
using System.Collections;
using System.Collections.Generic;

using System.Data.OleDb;

namespace TABS.Addons.CDRStores
{
    [NamedAddon("OleDb CDR Store", "Stores cdrs in an OleDb Table")]
    public class OleDbCdrStore : Extensibility.ICDRStore, IDisposable
    {
        protected string _ConfigString, _ConfigOptions;

        #region ICDRStore Members

        public string Name { get { return typeof(OleDbCdrStore).FullName; } }
        public string ConfigString { get { return _ConfigString; } set { _ConfigString = value; } }
        public string ConfigOptions { get { return _ConfigOptions; } set { _ConfigOptions = value; } }
        public string HelpHtml
        {
            get
            {
                return
                    @"Provides an OleDb Table CDR Store, i.e stores CDR Records in a Database Table with OleDb Connectivity. <br/>
                    The configuration string should be the Ole Db Connection String. <br/>
                    The Configuration Options should have the table name, in the format: <i>Table:<u>CDR</u></i>";
            }
        }
        public bool IsEnabled { get; set; }
        public OleDbCdrStore()
        {

        }
        public string Description { get; set; }
        // protected static DataConfiguration _DataConfigurationOledb = new DataConfiguration();
        protected OleDbCommand _insertCommand;

        static readonly string[] CDRFields = 
        { 
           "SwitchID",
           "IDonSwitch",
           "Tag",
           "AttemptDateTime",
           "AlertDateTime",
           "ConnectDateTime",
           "DisconnectDateTime",
           "DurationInSeconds",
           "IN_TRUNK",
           "IN_CIRCUIT",
           "IN_CARRIER",
           "IN_IP",
           "OUT_TRUNK",
           "OUT_CIRCUIT",
           "OUT_CARRIER",
           "OUT_IP",
           "CGPN",
           "CDPN",
           "CAUSE_FROM_RELEASE_CODE",
           "CAUSE_FROM",
           "CAUSE_TO_RELEASE_CODE",
           "CAUSE_TO"
        };

        /// <summary>
        /// Get Table name from ConfigOptions
        /// </summary>
        /// <returns></returns>
        protected string GetTableName()
        {
            // Get Table name from ConfigOptions
            return ConfigOptions;
        }

        /// <summary>
        /// Initialize the Ole Db Command (Text, Parameters)
        /// </summary>
        protected void InitializeCommands()
        {
            // Initialize Insert Command
            _insertCommand = new OleDbCommand();
            string tableName = GetTableName();
            string fieldNames = string.Join(",", CDRFields);
            //  string parameterNames = "@" + fieldNames.Replace(",", ",@");
            string parameterNames = "?";
            for (int i = 0; i < CDRFields.Length - 1; i++)
            {
                parameterNames = parameterNames + ",?";
            }

            _insertCommand.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, fieldNames, parameterNames);
            foreach (string fieldName in CDRFields) _insertCommand.Parameters.Add(new OleDbParameter("@" + fieldName, ""));
        }

        public void Put(NHibernate.ISession session, IEnumerable<CDR> cdrs)
        {
            using (OleDbConnection connection = new OleDbConnection(this.ConfigString))
            {
                connection.Open();

                if (_insertCommand == null) InitializeCommands();

                _insertCommand.Connection = connection;

                foreach (CDR cdr in cdrs)
                {
                    int i = 0;
                    _insertCommand.Parameters[i++].Value = cdr.Switch.SwitchID;
                    _insertCommand.Parameters[i++].Value = cdr.IDonSwitch;
                    _insertCommand.Parameters[i++].Value = cdr.Tag;
                    _insertCommand.Parameters[i++].Value = cdr.AttemptDateTime;
                    _insertCommand.Parameters[i++].Value = cdr.AlertDateTime;
                    _insertCommand.Parameters[i++].Value = cdr.ConnectDateTime;
                    _insertCommand.Parameters[i++].Value = cdr.DisconnectDateTime;
                    _insertCommand.Parameters[i++].Value = cdr.DurationInSeconds;
                    _insertCommand.Parameters[i++].Value = cdr.IN_TRUNK;
                    _insertCommand.Parameters[i++].Value = cdr.IN_CIRCUIT;
                    _insertCommand.Parameters[i++].Value = cdr.IN_CARRIER;
                    _insertCommand.Parameters[i++].Value = cdr.IN_IP;
                    _insertCommand.Parameters[i++].Value = cdr.OUT_TRUNK;
                    _insertCommand.Parameters[i++].Value = cdr.OUT_CIRCUIT;
                    _insertCommand.Parameters[i++].Value = cdr.OUT_CARRIER;
                    _insertCommand.Parameters[i++].Value = cdr.OUT_IP;
                    _insertCommand.Parameters[i++].Value = cdr.CGPN;
                    _insertCommand.Parameters[i++].Value = cdr.CDPN;
                    _insertCommand.Parameters[i++].Value = cdr.CAUSE_FROM_RELEASE_CODE;
                    _insertCommand.Parameters[i++].Value = cdr.CAUSE_FROM;
                    _insertCommand.Parameters[i++].Value = cdr.CAUSE_TO_RELEASE_CODE;
                    _insertCommand.Parameters[i++].Value = cdr.CAUSE_TO;

                    _insertCommand.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public IEnumerable<CDR> Get(NHibernate.ISession session, DateTime from, DateTime till)
        {
            OleDbConnection connection = new OleDbConnection(this.ConfigString);

            OleDbCommand readCommand = connection.CreateCommand();
            
            string fieldNames = string.Join(",", CDRFields);
            string tableName = GetTableName();
            readCommand.CommandText = string.Format("SELECT {0} From {1} where AttemptDateTime BETWEEN ?  AND  ? ", fieldNames, tableName);
            readCommand.Parameters.Add(new OleDbParameter("@from", from));
            readCommand.Parameters.Add(new OleDbParameter("@till", till));
            connection.Open();

            OleDbDataReader reader = readCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

            IEnumerable<CDR> cdrReader = new CdrReaderEnumerator(reader);

            return cdrReader;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_insertCommand != null) _insertCommand.Dispose();
        }

        #endregion

    }
}