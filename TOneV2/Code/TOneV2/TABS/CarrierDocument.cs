using System;

namespace TABS
{
    [Serializable]
    public class CarrierDocument : Components.BaseEntity
    {
        public override string Identifier { get { return "CarrierDocument:" + DocumentID; } }

        private int _DocumentID;
        private CarrierProfile _CarrierProfile;
        private string _Name;
        private string _Description;
        private string _Category;
        private byte[] _Document;
        private DateTime _Created = DateTime.Now;

        public virtual int DocumentID
        {
            get { return _DocumentID; }
            set { _DocumentID = value; }
        }

        public virtual string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public virtual string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public virtual string Category
        {
            get { return _Category; }
            set { _Category = value; }
        }

        public virtual byte[] Document
        {
            get { return _Document; }
            set { _Document = value; }
        }

        public virtual CarrierProfile CarrierProfile
        {
            get { return _CarrierProfile; }
            set { _CarrierProfile = value; }
        }

        public virtual DateTime Created
        {
            get { return _Created; }
            set { _Created = value; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}