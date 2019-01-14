
CREATE TABLE VR_AccountBalance_AccountUsage(
	ID bigint AUTO_INCREMENT NOT NULL,
	AccountTypeID char(38) NOT NULL,
	TransactionTypeID char(38) NOT NULL,
	AccountID varchar(50) NOT NULL,
	CurrencyId int NOT NULL,
	PeriodStart datetime(3) NOT NULL,
	PeriodEnd datetime(3) NOT NULL,
	UsageBalance decimal(20, 6) NOT NULL,
	IsOverridden bit NULL,
	OverriddenAmount decimal(20, 6) NULL,
	CorrectionProcessID char(38) NULL,
	CreatedTime datetime(3) NULL ,
	
 PRIMARY KEY 
(
	ID 
)
) ;

CREATE TABLE VR_AccountBalance_AccountUsageOverride(
	ID bigint AUTO_INCREMENT NOT NULL,
	AccountTypeID char(38) NOT NULL,
	TransactionTypeID char(38) NOT NULL,
	AccountID varchar(50) NOT NULL,
	PeriodStart datetime(3) NOT NULL,
	PeriodEnd datetime(3) NOT NULL,
	OverriddenByTransactionID bigint NOT NULL,
	CreatedTime datetime(3) NULL ,
	
 PRIMARY KEY
(
	ID 
)
);

CREATE TABLE VR_AccountBalance_BalanceClosingPeriod(
	ID bigint AUTO_INCREMENT NOT NULL,
	AccountTypeID char(38) NOT NULL,
	ClosingTime datetime(3) NOT NULL,
	CreatedTime datetime(3) NULL,
	
 PRIMARY KEY  
(
	ID 
)
) ;

CREATE TABLE VR_AccountBalance_BalanceHistory(
	ID bigint AUTO_INCREMENT NOT NULL,
	ClosingPeriodID bigint NOT NULL,
	AccountID varchar(50) NOT NULL,
	ClosingBalance decimal(20, 5) NOT NULL,
	CreatedTime datetime(3) NULL,
	
 PRIMARY KEY
(
	ID 
)
) ;

CREATE TABLE VR_AccountBalance_BalanceUsageQueue(
	ID bigint AUTO_INCREMENT NOT NULL,
	AccountTypeID char(38) NOT NULL,
	QueueType int NULL,
	UsageDetails LONGBLOB NOT NULL,
	CreatedTime datetime(3) NULL ,
 PRIMARY KEY
(
	ID 
)
) ;

CREATE TABLE VR_AccountBalance_BillingTransaction(
	ID bigint AUTO_INCREMENT NOT NULL,
	AccountTypeID char(38) NOT NULL,
	AccountID varchar(50) NOT NULL,
	TransactionTypeID char(38) NOT NULL,
	Amount decimal(20, 6) NOT NULL,
	CurrencyId int NOT NULL,
	TransactionTime datetime(3) NOT NULL,
	Reference nvarchar(255) NULL,
	Notes nvarchar(1000) NULL,
	CreatedByInvoiceID bigint NULL,
	IsBalanceUpdated bit NULL,
	ClosingPeriodId bigint NULL,
	Settings longtext NULL,
	CreatedTime datetime(3) NULL ,
	
	IsDeleted bit NULL,
	IsSubtractedFromBalance bit NULL,
	SourceID nvarchar(255) NULL,
 PRIMARY KEY
(
	ID 
)
) ;
CREATE TABLE VR_AccountBalance_BillingTransactionType(
	ID char(38) NOT NULL,
	Name nvarchar(255) NOT NULL,
	IsCredit bit NOT NULL,
	Settings longtext NOT NULL,
	CreatedTime datetime(3) NULL ,
	
 PRIMARY KEY 
(
	ID ASC
)
) ;

CREATE TABLE VR_AccountBalance_LiveBalance(
	ID bigint AUTO_INCREMENT NOT NULL,
	AccountTypeID char(38) NOT NULL,
	AccountID varchar(50) NOT NULL,
	CurrencyID int NOT NULL,
	InitialBalance decimal(20, 6) NOT NULL,
	CurrentBalance decimal(20, 6) NOT NULL,
	NextAlertThreshold decimal(20, 6) NULL,
	LastExecutedActionThreshold decimal(20, 6) NULL,
	AlertRuleID int NULL,
	ActiveAlertsInfo longtext NULL,
	BED datetime(3) NULL,
	EED datetime(3) NULL,
	Status int NULL,
	IsDeleted bit NULL,
	
	CreatedTime datetime(3) NULL ,
 PRIMARY KEY 
(
	ID 
)
) ;

CREATE TABLE VR_Invoice_BillingPeriodInfo(
	InvoiceTypeId char(38) NOT NULL,
	PartnerId varchar(50) NOT NULL,
	NextPeriodStart datetime(3) NOT NULL,
	CreatedTime datetime(3) NOT NULL ,
 PRIMARY KEY
(
	InvoiceTypeId ASC,
	PartnerId ASC
)
) ;

CREATE TABLE VR_Invoice_Invoice(
	ID bigint AUTO_INCREMENT NOT NULL,
	UserId int NOT NULL,
	InvoiceTypeID char(38) NOT NULL,
	PartnerID varchar(50) NOT NULL,
	SettlementInvoiceId bigint NULL,
	SplitInvoiceGroupId char(38) NULL,
	InvoiceSettingID char(38) NULL,
	SerialNumber nvarchar(255) NOT NULL,
	FromDate datetime(3) NOT NULL,
	ToDate datetime(3) NOT NULL,
	IssueDate date NOT NULL,
	DueDate date NULL,
	Details longtext NULL,
	PaidDate datetime(3) NULL,
	LockDate datetime(3) NULL,
	SentDate datetime(3) NULL,
	IsDeleted bit NOT NULL ,
	Notes longtext NULL,
	Settings longtext NULL,
	SourceId nvarchar(50) NULL,
	IsDraft bit NULL,
	IsAutomatic bit NULL,
	NeedApproval bit NULL,
	ApprovedTime datetime(3) NULL,
	ApprovedBy int NULL,
	CreatedTime datetime(3) NULL,
 PRIMARY KEY 
(
	ID 
)
);

CREATE TABLE VR_Invoice_InvoiceAccount(
	ID bigint AUTO_INCREMENT NOT NULL,
	InvoiceTypeID char(38) NOT NULL,
	PartnerID varchar(50) NOT NULL,
	BED datetime(3) NULL,
	EED datetime(3) NULL,
	Status int NOT NULL,
	IsDeleted bit NULL,
	CreatedTime datetime(3) NULL ,
	
 PRIMARY KEY
(
	ID 
)
) ;
CREATE TABLE VR_Invoice_InvoiceBulkActionDraft(
	ID bigint AUTO_INCREMENT NOT NULL,
	InvoiceBulkActionIdentifier char(38) NULL,
	InvoiceTypeId char(38) NULL,
	InvoiceId bigint NULL,
	CreatedTime datetime(3) NULL,
	PRIMARY KEY
(
	ID 
)
) ;

CREATE TABLE VR_Invoice_InvoiceGenerationDraft(
	ID bigint AUTO_INCREMENT NOT NULL,
	InvoiceGenerationIdentifier char(38) NULL,
	InvoiceTypeId char(38) NOT NULL,
	PartnerID varchar(50) NOT NULL,
	PartnerName longtext NOT NULL,
	FromDate datetime(3) NOT NULL,
	ToDate datetime(3) NOT NULL,
	CustomPayload longtext NULL,
	CreatedTime datetime(3) NULL ,
 PRIMARY KEY
(
	ID 
)
) ;

CREATE TABLE VR_Invoice_InvoiceItem(
	ID bigint AUTO_INCREMENT NOT NULL,
	InvoiceID bigint NOT NULL,
	ItemSetName nvarchar(255) NULL,
	Name nvarchar(900) NOT NULL,
	Details longtext NULL,
	CreatedTime datetime(3) NULL ,
 PRIMARY KEY
(
	ID 
)
) ;

CREATE TABLE VR_Invoice_InvoiceReportFile(
	ID char(38) NOT NULL,
	Name nvarchar(255) NULL,
	ReportName nvarchar(255) NULL,
	InvoiceTypeId char(38) NULL,
	CreatedTime datetime(3) NULL ,
	CreatedBy int NULL,
	LastModifiedTime datetime(3) NULL,
	LastModifiedBy int NULL,
	
 PRIMARY KEY
(
	ID ASC
)
);

CREATE TABLE VR_Invoice_InvoiceSequence(
	SequenceGroup varchar(255) NULL,
	InvoiceTypeID char(38) NOT NULL,
	SequenceKey nvarchar(255) NOT NULL,
	InitialValue bigint NOT NULL,
	LastValue bigint NOT NULL,
	CreatedTime datetime(3) NULL ,
	
 PRIMARY KEY
(
	InvoiceTypeID ASC,
	SequenceKey ASC
)
) ;

CREATE TABLE VR_Invoice_InvoiceSetting(
	ID char(38) NOT NULL,
	InvoiceTypeId char(38) NULL,
	Name varchar(50) NULL,
	IsDefault bit NULL,
	Details longtext NULL,
	IsDeleted bit NULL,
	CreatedTime datetime(3) NULL ,
	
 PRIMARY KEY
(
	ID ASC
)
) ;

CREATE TABLE VR_Invoice_InvoiceType(
	ID char(38) NOT NULL,
	Name nvarchar(255) NOT NULL,
	Settings longtext NULL,
	
	CreatedTime datetime(3) NULL ,
 PRIMARY KEY
(
	ID ASC
)
) ;

CREATE TABLE VR_Invoice_PartnerInvoiceSetting(
	ID char(38) NOT NULL,
	PartnerID varchar(50) NOT NULL,
	InvoiceSettingID char(38) NOT NULL,
	Details longtext NULL,
	CreatedTime datetime(3) NULL ,
	
 PRIMARY KEY
(
	ID ASC
)
) ;