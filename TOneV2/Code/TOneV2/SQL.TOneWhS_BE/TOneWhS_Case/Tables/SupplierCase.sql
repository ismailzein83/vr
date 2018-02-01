CREATE TABLE [TOneWhS_Case].[SupplierCase] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CaseTime]         DATETIME         CONSTRAINT [DF_SupplierCase_CaseTime] DEFAULT (getdate()) NULL,
    [SupplierId]       INT              NULL,
    [SupplierZoneId]   BIGINT           NULL,
    [FromDate]         DATETIME         NULL,
    [ToDate]           DATETIME         NULL,
    [Attempts]         INT              NULL,
    [ASR]              DECIMAL (20, 8)  NULL,
    [ACD]              DECIMAL (20, 8)  NULL,
    [CarrierReference] NVARCHAR (255)   NULL,
    [Description]      NVARCHAR (1000)  NULL,
    [Notes]            NVARCHAR (1000)  NULL,
    [StatusId]         UNIQUEIDENTIFIER NULL,
    [Attachments]      NVARCHAR (MAX)   NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [ContactName]      NVARCHAR (255)   NULL,
    [PhoneNumber]      NVARCHAR (255)   NULL,
    [ContactEmails]    NVARCHAR (1000)  NULL,
    CONSTRAINT [PK_SupplierCase] PRIMARY KEY CLUSTERED ([ID] ASC)
);



