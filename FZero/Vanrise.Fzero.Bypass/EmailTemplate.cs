//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vanrise.Fzero.Bypass
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmailTemplate
    {
        public EmailTemplate()
        {
            this.Emails = new HashSet<Email>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string MessageBody { get; set; }
        public string Subject { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime LastUpdateDate { get; set; }
        public int LastUpdatedBy { get; set; }
        public Nullable<int> AppPortal { get; set; }
    
        public virtual ICollection<Email> Emails { get; set; }
        public virtual User User { get; set; }
    }
}
