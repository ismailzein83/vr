
CREATE TABLE "VR_AccountBalance"."AccountUsage"(
	"ID" bigserial NOT NULL,
	"AccountTypeID" uuid NOT NULL,
	"TransactionTypeID" uuid NOT NULL,
	"AccountID" varying(50)[] NOT NULL,
	"CurrencyId" integer NOT NULL,
	"PeriodStart" timestamp without time zone NOT NULL,
	"PeriodEnd" timestamp without time zone NOT NULL,
	"UsageBalance" numeric(20, 6) NOT NULL,
	"IsOverridden" boolean NULL,
	"OverriddenAmount" numeric(20, 6) NULL,
	"CorrectionProcessID" uuid NULL,
	"CreatedTime" timestamp without time zone NULL ,
 PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_AccountBalance"."AccountUsageOverride"(
	"ID" bigserial NOT NULL,
	"AccountTypeID" uuid NOT NULL,
	"TransactionTypeID" uuid NOT NULL,
	"AccountID" varying(50)[] NOT NULL,
	"PeriodStart" timestamp without time zone NOT NULL,
	"PeriodEnd" timestamp without time zone NOT NULL,
	"OverriddenByTransactionID" bigint NOT NULL,
	"CreatedTime" timestamp without time zone NULL,
  PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_AccountBalance"."BalanceClosingPeriod"(
	"ID" bigserial NOT NULL,
	"AccountTypeID" uuid NOT NULL,
	"ClosingTime" timestamp without time zone NOT NULL,
	"CreatedTime" timestamp without time zone NULL,
   PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_AccountBalance"."BalanceHistory"(
	"ID" bigserial NOT NULL,
	"ClosingPeriodID" bigint NOT NULL,
	"AccountID" varying(50)[] NOT NULL,
	"ClosingBalance" numeric(20, 5) NOT NULL,
	"CreatedTime" timestamp without time zone NULL,
	"timestamp" "timestamp" NULL,
  PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_AccountBalance"."BalanceUsageQueue"(
	"ID" bigserial NOT NULL,
	"AccountTypeID" uuid NOT NULL,
	"QueueType" integer NULL,
	"UsageDetails" bytea NOT NULL,
	"CreatedTime" timestamp without time zone NULL ,
 PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_AccountBalance"."BillingTransaction"(
	"ID" bigserial NOT NULL,
	"AccountTypeID" uuid NOT NULL,
	"AccountID" varying(50)[] NOT NULL,
	"TransactionTypeID" uuid NOT NULL,
	"Amount" numeric(20, 6) NOT NULL,
	"CurrencyId" integer NOT NULL,
	"TransactionTime" timestamp without time zone NOT NULL,
	"Reference" varying(255)[] NULL,
	"Notes" varying(1000)[] NULL,
	"CreatedByInvoiceID" bigint NULL,
	"IsBalanceUpdated" boolean NULL,
	"ClosingPeriodId" bigint NULL,
	"Settings" varying NULL,
	"CreatedTime" timestamp without time zone NULL ,
	"IsDeleted" boolean NULL,
	"IsSubtractedFromBalance" boolean NULL,
	"SourceID" varying(255)[] NULL,
  PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_AccountBalance"."BillingTransactionType"(
	"ID" uuid NOT NULL,
	"Name" varying(255)[] NOT NULL,
	"IsCredit" boolean NOT NULL,
	"Settings" varying NOT NULL,
	"CreatedTime" timestamp without time zone NULL ,
  PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_AccountBalance"."LiveBalance"(
	"ID" bigserial NOT NULL,
	"AccountTypeID" uuid NOT NULL,
	"AccountID" varying(50)[] NOT NULL,
	"CurrencyID" integer NOT NULL,
	"InitialBalance" numeric(20, 6) NOT NULL,
	"CurrentBalance" numeric(20, 6) NOT NULL,
	"NextAlertThreshold" numeric(20, 6) NULL,
	"LastExecutedActionThreshold" numeric(20, 6) NULL,
	"AlertRuleID" integer NULL,
	"ActiveAlertsInfo" varying NULL,
	"BED" timestamp without time zone NULL,
	"EED" timestamp without time zone NULL,
	"Status" integer NULL,
	"IsDeleted" boolean NULL,
	"CreatedTime" timestamp without time zone NULL ,
  PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_Invoice"."BillingPeriodInfo"(
	"InvoiceTypeId" uuid NOT NULL,
	"PartnerId" varying(50)[] NOT NULL,
	"NextPeriodStart" timestamp without time zone NOT NULL,
	"CreatedTime" timestamp without time zone NOT NULL ,
 PRIMARY KEY  
(
	"InvoiceTypeId" ,
	"PartnerId" 
)
) 

CREATE TABLE "VR_Invoice"."Invoice"(
	"ID" bigserial NOT NULL,
	"UserId" integer NOT NULL,
	"InvoiceTypeID" uuid NOT NULL,
	"PartnerID" varying(50)[] NOT NULL,
	"SettlementInvoiceId" bigint NULL,
	"SplitInvoiceGroupId" uuid NULL,
	"InvoiceSettingID" uuid NULL,
	"SerialNumber" varying(255)[] NOT NULL,
	"FromDate" timestamp without time zone NOT NULL,
	"ToDate" timestamp without time zone NOT NULL,
	"IssueDate" "date" NOT NULL,
	"DueDate" "date" NULL,
	"Details" varying NULL,
	"PaidDate" timestamp without time zone NULL,
	"LockDate" timestamp without time zone NULL,
	"SentDate" timestamp without time zone NULL,
	"IsDeleted" boolean NOT NULL ,
	"Notes" varying NULL,
	"Settings" varying NULL,
	"SourceId" varying(50)[] NULL,
	"IsDraft" boolean NULL,
	"IsAutomatic" boolean NULL,
	"NeedApproval" boolean NULL,
	"ApprovedTime" timestamp without time zone NULL,
	"ApprovedBy" integer NULL,
	"CreatedTime" timestamp without time zone NULL ,
  PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_Invoice"."InvoiceAccount"(
	"ID" bigserial NOT NULL,
	"InvoiceTypeID" uuid NOT NULL,
	"PartnerID" varying(50)[] NOT NULL,
	"BED" timestamp without time zone NULL,
	"EED" timestamp without time zone NULL,
	"Status" integer NOT NULL,
	"IsDeleted" boolean NULL,
	"CreatedTime" timestamp without time zone NULL ,
	"timestamp" "timestamp" NULL,
  PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_Invoice"."InvoiceBulkActionDraft"(
	"ID" bigserial NOT NULL,
	"InvoiceBulkActionIdentifier" uuid NULL,
	"InvoiceTypeId" uuid NULL,
	"InvoiceId" bigint NULL,
	"CreatedTime" timestamp without time zone NULL 
) 

CREATE TABLE "VR_Invoice"."InvoiceGenerationDraft"(
	"ID" bigserial NOT NULL,
	"InvoiceGenerationIdentifier" uuid NULL,
	"InvoiceTypeId" uuid NOT NULL,
	"PartnerID" varying(50)[] NOT NULL,
	"PartnerName" varying NOT NULL,
	"FromDate" timestamp without time zone NOT NULL,
	"ToDate" timestamp without time zone NOT NULL,
	"CustomPayload" varying NULL,
	"CreatedTime" timestamp without time zone NULL ,
  PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_Invoice"."InvoiceItem"(
	"ID" bigserial NOT NULL,
	"InvoiceID" bigint NOT NULL,
	"ItemSetName" varying(255)[] NULL,
	"Name" varying(900)[] NOT NULL,
	"Details" varying NULL,
	"CreatedTime" timestamp without time zone NULL ,
   PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_Invoice"."InvoiceReportFile"(
	"ID" uuid NOT NULL,
	"Name" varying(255)[] NULL,
	"ReportName" varying(255)[] NULL,
	"InvoiceTypeId" uuid NULL,
	"CreatedTime" timestamp without time zone NULL ,
	"CreatedBy" integer NULL,
	"LastModifiedTime" timestamp without time zone NULL,
	"LastModifiedBy" integer NULL,
PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_Invoice"."InvoiceSequence"(
	"SequenceGroup" varying(255)[] NULL,
	"InvoiceTypeID" uuid NOT NULL,
	"SequenceKey" varying(255)[] NOT NULL,
	"InitialValue" bigint NOT NULL,
	"LastValue" bigint NOT NULL,
	"CreatedTime" timestamp without time zone NULL ,
  PRIMARY KEY  
(
	"InvoiceTypeID" ,
	"SequenceKey" 
)
) 

CREATE TABLE "VR_Invoice"."InvoiceSetting"(
	"ID" uuid NOT NULL,
	"InvoiceTypeId" uuid NULL,
	"Name" varying(50)[] NULL,
	"IsDefault" boolean NULL,
	"Details" varying NULL,
	"IsDeleted" boolean NULL,
	"CreatedTime" timestamp without time zone NULL ,
  PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_Invoice"."InvoiceType"(
	"ID" uuid NOT NULL,
	"Name" varying(255)[] NOT NULL,
	"Settings" varying NULL,
	"timestamp" "timestamp" NULL,
	"CreatedTime" timestamp without time zone NULL ,
 PRIMARY KEY  
(
	"ID" 
)
) 

CREATE TABLE "VR_Invoice"."PartnerInvoiceSetting"(
	"ID" uuid NOT NULL,
	"PartnerID" varying(50)[] NOT NULL,
	"InvoiceSettingID" uuid NOT NULL,
	"Details" varying NULL,
	"CreatedTime" timestamp without time zone NULL,
	"timestamp" "timestamp" NULL,
  PRIMARY KEY  
(
	"ID" 
)
) 
