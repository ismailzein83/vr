
CREATE TABLE VRAcBal_AccountUsage(
	ID Number(19)  NOT NULL,
	AccountTypeID RAW(16) NOT NULL,
	TransactionTypeID RAW(16) NOT NULL,
	AccountID varchar(50) NOT NULL,
	CurrencyId int NOT NULL,
	PeriodStart TIMESTAMP (6) NOT NULL,
	PeriodEnd TIMESTAMP (6) NOT NULL,
	UsageBalance number(20, 6) NOT NULL,
	IsOverridden NUMBER(1) NULL,
	OverriddenAmount number(20, 6) NULL,
	CorrectionProcessID RAW(16) NULL,
	CreatedTime TIMESTAMP (6) NULL ,
	
 PRIMARY KEY 
(
	ID 
)
) ;

CREATE TABLE VRAcBal_AccountUsageOverride(
	ID Number(19)  NOT NULL,
	AccountTypeID RAW(16) NOT NULL,
	TransactionTypeID RAW(16) NOT NULL,
	AccountID varchar(50) NOT NULL,
	PeriodStart TIMESTAMP (6) NOT NULL,
	PeriodEnd TIMESTAMP (6) NOT NULL,
	OverriddenByTransactionID Number(19) NOT NULL,
	CreatedTime TIMESTAMP (6) NULL ,
	
 PRIMARY KEY
(
	ID 
)
);

CREATE TABLE VRAcBal_BalanceClosingPeriod(
	ID Number(19)  NOT NULL,
	AccountTypeID RAW(16) NOT NULL,
	ClosingTime TIMESTAMP (6) NOT NULL,
	CreatedTime TIMESTAMP (6) NULL,
	
 PRIMARY KEY  
(
	ID 
)
) ;

CREATE TABLE VRAcBal_BalanceHistory(
	ID Number(19)  NOT NULL,
	ClosingPeriodID Number(19) NOT NULL,
	AccountID varchar(50) NOT NULL,
	ClosingBalance number(20, 5) NOT NULL,
	CreatedTime TIMESTAMP (6) NULL,
	
 PRIMARY KEY
(
	ID 
)
) ;

CREATE TABLE VRAcBal_BalanceUsageQueue(
	ID Number(19)  NOT NULL,
	AccountTypeID RAW(16) NOT NULL,
	QueueType int NULL,
	UsageDetails BLOB NOT NULL,
	CreatedTime TIMESTAMP (6) NULL ,
 PRIMARY KEY
(
	ID 
)
) ;

CREATE TABLE VRAcBal_BillingTransaction(
	ID Number(19)  NOT NULL,
	AccountTypeID RAW(16) NOT NULL,
	AccountID varchar(50) NOT NULL,
	TransactionTypeID RAW(16) NOT NULL,
	Amount number(20, 6) NOT NULL,
	CurrencyId int NOT NULL,
	TransactionTime TIMESTAMP (6) NOT NULL,
	Reference varchar2(255 char) NULL,
	Notes varchar2(1000 char) NULL,
	CreatedByInvoiceID Number(19) NULL,
	IsBalanceUpdated NUMBER(1) NULL,
	ClosingPeriodId Number(19) NULL,
	Settings NCLOB NULL,
	CreatedTime TIMESTAMP (6) NULL ,
	
	IsDeleted NUMBER(1) NULL,
	IsSubtractedFromBalance NUMBER(1) NULL,
	SourceID varchar2(255 char) NULL,
 PRIMARY KEY
(
	ID 
)
) ;
CREATE TABLE VRAcBal_BillingTransactionType(
	ID RAW(16) NOT NULL,
	Name varchar2(255 char) NOT NULL,
	IsCredit NUMBER(1) NOT NULL,
	Settings NCLOB NOT NULL,
	CreatedTime TIMESTAMP (6) NULL ,
	
 PRIMARY KEY 
(
	ID 
)
) ;

CREATE TABLE VRAcBal_LiveBalance(
	ID Number(19)  NOT NULL,
	AccountTypeID RAW(16) NOT NULL,
	AccountID varchar(50) NOT NULL,
	CurrencyID int NOT NULL,
	InitialBalance number(20, 6) NOT NULL,
	CurrentBalance number(20, 6) NOT NULL,
	NextAlertThreshold number(20, 6) NULL,
	LastExecutedActionThreshold number(20, 6) NULL,
	AlertRuleID int NULL,
	ActiveAlertsInfo NCLOB NULL,
	BED TIMESTAMP (6) NULL,
	EED TIMESTAMP (6) NULL,
	Status int NULL,
	IsDeleted NUMBER(1) NULL,
	
	CreatedTime TIMESTAMP (6) NULL ,
 PRIMARY KEY 
(
	ID 
)
) ;

CREATE TABLE VRInv_BillingPeriodInfo(
	InvoiceTypeId RAW(16) NOT NULL,
	PartnerId varchar(50) NOT NULL,
	NextPeriodStart TIMESTAMP (6) NOT NULL,
	CreatedTime TIMESTAMP (6) NOT NULL ,
 PRIMARY KEY
(
	InvoiceTypeId ,
	PartnerId 
)
) ;

CREATE TABLE VRInv_Invoice(
	ID Number(19)  NOT NULL,
	UserId int NOT NULL,
	InvoiceTypeID RAW(16) NOT NULL,
	PartnerID varchar(50) NOT NULL,
	SettlementInvoiceId Number(19) NULL,
	SplitInvoiceGroupId RAW(16) NULL,
	InvoiceSettingID RAW(16) NULL,
	SerialNumber varchar2(255 char) NOT NULL,
	FromDate TIMESTAMP (6) NOT NULL,
	ToDate TIMESTAMP (6) NOT NULL,
	IssueDate TIMESTAMP (6) NOT NULL,
	DueDate TIMESTAMP (6) NULL,
	Details NCLOB NULL,
	PaidDate TIMESTAMP (6) NULL,
	LockDate TIMESTAMP (6) NULL,
	SentDate TIMESTAMP (6) NULL,
	IsDeleted NUMBER(1) NOT NULL ,
	Notes NCLOB NULL,
	Settings NCLOB NULL,
	SourceId varchar2(50 char) NULL,
	IsDraft NUMBER(1) NULL,
	IsAutomatic NUMBER(1) NULL,
	NeedApproval NUMBER(1) NULL,
	ApprovedTime TIMESTAMP (6) NULL,
	ApprovedBy int NULL,
	CreatedTime TIMESTAMP (6) NULL,
 PRIMARY KEY 
(
	ID 
)
);

CREATE TABLE VRInv_InvoiceAccount(
	ID Number(19)  NOT NULL,
	InvoiceTypeID RAW(16) NOT NULL,
	PartnerID varchar(50) NOT NULL,
	BED TIMESTAMP (6) NULL,
	EED TIMESTAMP (6) NULL,
	Status int NOT NULL,
	IsDeleted NUMBER(1) NULL,
	CreatedTime TIMESTAMP (6) NULL ,
	
 PRIMARY KEY
(
	ID 
)
) ;
CREATE TABLE VRInv_InvoiceBulkActionDraft(
	ID Number(19)  NOT NULL,
	InvoiceBulkActionIdentifier RAW(16) NULL,
	InvoiceTypeId RAW(16) NULL,
	InvoiceId Number(19) NULL,
	CreatedTime TIMESTAMP (6) NULL,
	PRIMARY KEY
(
	ID 
)
) ;

CREATE TABLE VRInv_InvoiceGenerationDraft(
	ID Number(19)  NOT NULL,
	InvoiceGenerationIdentifier RAW(16) NULL,
	InvoiceTypeId RAW(16) NOT NULL,
	PartnerID varchar(50) NOT NULL,
	PartnerName NCLOB NOT NULL,
	FromDate TIMESTAMP (6) NOT NULL,
	ToDate TIMESTAMP (6) NOT NULL,
	CustomPayload NCLOB NULL,
	CreatedTime TIMESTAMP (6) NULL ,
 PRIMARY KEY
(
	ID 
)
) ;

CREATE TABLE VRInv_InvoiceItem(
	ID Number(19)  NOT NULL,
	InvoiceID Number(19) NOT NULL,
	ItemSetName varchar2(255 char) NULL,
	Name varchar2(900 char) NOT NULL,
	Details NCLOB NULL,
	CreatedTime TIMESTAMP (6) NULL ,
 PRIMARY KEY
(
	ID 
)
) ;

CREATE TABLE VRInv_InvoiceReportFile(
	ID RAW(16) NOT NULL,
	Name varchar2(255 char) NULL,
	ReportName varchar2(255 char) NULL,
	InvoiceTypeId RAW(16) NULL,
	CreatedTime TIMESTAMP (6) NULL ,
	CreatedBy int NULL,
	LastModifiedTime TIMESTAMP (6) NULL,
	LastModifiedBy int NULL,
	
 PRIMARY KEY
(
	ID 
)
);

CREATE TABLE VRInv_InvoiceSequence(
	SequenceGroup varchar(255) NULL,
	InvoiceTypeID RAW(16) NOT NULL,
	SequenceKey varchar2(255 char) NOT NULL,
	InitialValue Number(19) NOT NULL,
	LastValue Number(19) NOT NULL,
	CreatedTime TIMESTAMP (6) NULL ,
	
 PRIMARY KEY
(
	InvoiceTypeID ,
	SequenceKey 
)
) ;

CREATE TABLE VRInv_InvoiceSetting(
	ID RAW(16) NOT NULL,
	InvoiceTypeId RAW(16) NULL,
	Name varchar(50) NULL,
	IsDefault NUMBER(1) NULL,
	Details NCLOB NULL,
	IsDeleted NUMBER(1) NULL,
	CreatedTime TIMESTAMP (6) NULL ,
	
 PRIMARY KEY
(
	ID 
)
) ;

CREATE TABLE VRInv_InvoiceType(
	ID RAW(16) NOT NULL,
	Name varchar2(255 char) NOT NULL,
	Settings NCLOB NULL,
	
	CreatedTime TIMESTAMP (6) NULL ,
 PRIMARY KEY
(
	ID 
)
) ;

CREATE TABLE VRInv_PartnerInvoiceSetting(
	ID RAW(16) NOT NULL,
	PartnerID varchar(50) NOT NULL,
	InvoiceSettingID RAW(16) NOT NULL,
	Details NCLOB NULL,
	CreatedTime TIMESTAMP (6) NULL ,
	
 PRIMARY KEY
(
	ID 
)
) ;