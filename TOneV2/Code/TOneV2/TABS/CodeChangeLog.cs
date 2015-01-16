using System;
using System.Collections.Generic;

namespace TABS
{
    [Serializable]
    public class CodeChangeLog : Components.BaseEntity
    {
        public CodeChangeLog()
        {
            this.Updated = DateTime.Today;
        }

        #region Data Members

        private int _ID;
        private string _Description;
        private byte[] _SourceFileBytes;
        private string _SourceFileName;
        private bool _IsRestored;
        private int _StateBackUpID;
        public DateTime Updated { get; set; }

        public virtual int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public virtual string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public virtual string SourceFileName
        {
            get { return _SourceFileName; }
            set { _SourceFileName = value; }
        }

        public virtual byte[] SourceFileBytes
        {
            get { return _SourceFileBytes; }
            set { _SourceFileBytes = value; }
        }

        public virtual bool IsRestored
        {
            get { return _IsRestored; }
            set { _IsRestored = value; }
        }

        public virtual int StateBackUpID
        {
            get { return _StateBackUpID; }
            set { _StateBackUpID = value; }
        }

        #endregion Data Members

        #region Business Members

        public static Dictionary<int, CodeChangeLog> All
        {
            get { return ObjectAssembler.GetCodeChangeLogs(); }
        }

        #endregion Business Members




        public override string Identifier
        {
            get { return "CodeChangeLog" + ID; }
        }
    }
}
