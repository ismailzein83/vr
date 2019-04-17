﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public enum ContractDetailStatus { Active, Inactive };

    public class ContractDetail
    {
        public string ContractId { get; set; }

        public string CustomerId { get; set; }

        public string PhoneNumber { get; set; }

        public string RatePlanId { get; set; }

        public string RatePlanName { get; set; }

        public string Address { get; set; }

        public Address ContractAddress { get; set; }

        public string CSO { get; set; }

        public string SubscriptionDate { get; set; }

        public string ReservedLinePath { get; set; }

        public ContractDetailStatus Status { get; set; }

        public DateTime? ActivationDate { get; set; }

        public DateTime StatusDate { get; set; }

        public decimal ContractBalance { get; set; }

        public decimal UnbilledAmount { get; set; }

        public string Promotions { get; set; }

        public string FreeUnit { get; set; }

        public string PathId { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? LastModifiedTime { get; set; }

        public string CustomerCode {get;set;}
    }

    public class ContractInfo
    {
        public string ContractId { get; set; }

        public string CustomerId { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

    }


}
