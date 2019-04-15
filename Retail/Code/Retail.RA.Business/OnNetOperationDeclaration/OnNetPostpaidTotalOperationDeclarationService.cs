﻿using Retail.RA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Business
{
    public class OnNetPostpaidTotalOperationDeclarationService : OnNetOperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("69D309FE-A74C-461C-9981-76D8A701F5BE"); }
        }
        public int? NumberOfSubscribers { get; set; }
        public decimal? MonthlyCharges { get; set; }
        public decimal? Revenue { get; set; }
    }
    public class OnNetPostpaidVoiceOperationDeclarationService : OnNetOperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("962EFE0D-0A2A-4CBB-BFD2-EC9D85BBD529"); }
        }
        public int? Calls { get; set; }
        public decimal? Revenue { get; set; }
        public decimal? Duration { get; set; }
        public decimal? RevenueExcludingBundles { get; set; }
        public int? DurationExcludingBundles { get; set; }
        public Scope Scope { get; set; }
    }
    public class OnNetPostpaidSMSOperationDeclarationService : OnNetOperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("9C5DD4B4-827A-4541-A032-3BAF780557D4"); }
        }
        public decimal? Revenue { get; set; }
        public int? SMS { get; set; }
        public decimal? RevenueExcludingBundles { get; set; }
        public int? SMSExcludingBundles { get; set; }
        public Scope Scope { get; set; }
    }
    public class OnNetPrepaidTotalOperationDeclarationService : OnNetOperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("DFEBE67A-633A-4736-8B95-873125B703C8"); }
        }
        public int? NumberOfSubscribers { get; set; }
        public decimal? AmountFromTopups { get; set; }
        public decimal? ResidualAmountFromTopups { get; set; }
    }
    public class OnNetPrepaidVoiceOperationDeclarationService : OnNetOperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("3B02F297-DDF9-440C-A2DF-95D349F46DF4"); }
        }
        public int? Calls { get; set; }
        public decimal? Revenue { get; set; }
        public decimal? Duration { get; set; }
        public decimal? RevenueExcludingBundles { get; set; }
        public int? DurationExcludingBundles { get; set; }
        public Scope Scope { get; set; }
    }
    public class OnNetPrepaidSMSOperationDeclarationService : OnNetOperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("AC62ADF8-DFB8-4A72-B072-9CB12DC6E197"); }
        }
        public decimal? Revenue { get; set; }
        public int? SMS { get; set; }
        public decimal? RevenueExcludingBundles { get; set;}
        public int? SMSExcludingBundles { get; set; }
        public Scope Scope { get; set; }
    }

}
