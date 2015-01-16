using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABS
{
    public class OnlinePaymentTransaction
    {
        private int _transactionID;
        private string _carrierID;
        private string _userRef;
        protected string _message;
        protected DateTime _transactionDate;
        protected string _expireDate;
        protected string _status;
        protected string _cardNumber;
        protected float _amount;
        public virtual int TransactionID
        {
            get { return _transactionID; }
            set { _transactionID = value; }
        }


        public virtual string CarrierID
        {
            get { return _carrierID; }
            set { _carrierID = value; }
        }


        public virtual string UserRef
        {
            get { return _userRef; }
            set { _userRef = value; }
        }


        public virtual string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public virtual DateTime TransactionDate
        {
            get { return _transactionDate; }
            set { _transactionDate = value; }
        }
        public virtual string ExpireDate
        {
            get { return _expireDate; }
            set { _expireDate = value; }
        }
        public virtual string Status
        {
            get { return _status; }
            set { _status = value; }
        }
        public virtual string CardNumber
        {
            get { return _cardNumber; }
            set { _cardNumber = value; }
        }

        public virtual float Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
    }
}
