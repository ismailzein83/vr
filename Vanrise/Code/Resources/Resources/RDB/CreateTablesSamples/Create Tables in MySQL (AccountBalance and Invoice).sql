CREATE TABLE VR_Invoice_BillingPeriodInfo(
	InvoiceTypeId char(38) PRIMARY KEY NOT NULL,
	PartnerId varchar(50) PRIMARY KEY NOT NULL,
	NextPeriodStart datetime NOT NULL,
	CreatedTime datetime NOT NULL ) ;

CREATE TABLE VR_Invoice_InvoiceAccount(
	ID bigint AUTO_INCREMENT PRIMARY KEY NOT NULL,
	InvoiceTypeID char(38) NOT NULL,
	PartnerID varchar(50) NOT NULL,
	BED datetime NULL,
	EED datetime NULL,
	Status int NOT NULL,
	IsDeleted bit NULL,
	CreatedTime datetime NULL ) ;


CREATE TABLE VR_Invoice_InvoiceBulkActionDraft(
	ID bigint AUTO_INCREMENT NOT NULL,
	InvoiceBulkActionIdentifier char(38) NULL,
	InvoiceTypeId char(38) NULL,
	InvoiceId bigint NULL,
	CreatedTime datetime 
) ;


CREATE TABLE VR_Invoice_InvoiceGenerationDraft(
	ID bigint AUTO_INCREMENT PRIMARY KEY NOT NULL,
	InvoiceGenerationIdentifier char(38) NULL,
	InvoiceTypeId char(38) NOT NULL,
	PartnerID varchar(50) NOT NULL,
	PartnerName text NOT NULL,
	FromDate datetime NOT NULL,
	ToDate datetime NOT NULL,
	CustomPayload text NULL,
	CreatedTime datetime NULL) ;


CREATE TABLE VR_Invoice_InvoiceItem(
	ID bigint AUTO_INCREMENT PRIMARY KEY NOT NULL,
	InvoiceID bigint NOT NULL,
	ItemSetName nvarchar(255) NULL,
	Name nvarchar(900) NOT NULL,
	Details text NULL,
	CreatedTime datetime NULL ) ;


CREATE TABLE VR_Invoice_InvoiceSequence(
	SequenceGroup varchar(255) NULL,
	InvoiceTypeID char(38) PRIMARY KEY NOT NULL,
	SequenceKey nvarchar(255) PRIMARY KEY NOT NULL,
	InitialValue bigint NOT NULL,
	LastValue bigint NOT NULL,
	CreatedTime datetime NULL );


CREATE TABLE VR_Invoice_InvoiceSetting(
	ID char(38) PRIMARY KEY NOT NULL,
	InvoiceTypeId char(38) NULL,
	Name varchar(50) NULL,
	IsDefault bit NULL,
	Details text NULL,
	IsDeleted bit NULL,
	CreatedTime datetime NULL) ;


CREATE TABLE VR_Invoice_InvoiceType(
	ID char(38) PRIMARY KEY NOT NULL,
	Name nvarchar(255) NOT NULL,
	Settings text NULL,
	timestamp timestamp NULL,
	CreatedTime datetime NULL ) ;


CREATE TABLE VR_Invoice_PartnerInvoiceSetting(
	ID char(38) PRIMARY KEY NOT NULL,
	PartnerID varchar(50) NOT NULL,
	InvoiceSettingID char(38) NOT NULL,
	Details text NULL,
	CreatedTime datetime NULL) 
