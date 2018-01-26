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
    CONSTRAINT [PK_SupplierCase] PRIMARY KEY CLUSTERED ([ID] ASC)
);

