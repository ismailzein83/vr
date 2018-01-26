CREATE TABLE [TOneWhS_Case].[CustomerCase] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [OwnerReference]   NVARCHAR (1000)  NOT NULL,
    [CaseTime]         DATETIME         CONSTRAINT [DF_CustomerCase_CaseTime] DEFAULT (getdate()) NULL,
    [FromDate]         DATETIME         NULL,
    [ToDate]           DATETIME         NULL,
    [Attempts]         INT              NULL,
    [ASR]              DECIMAL (20, 8)  NULL,
    [ACD]              DECIMAL (20, 8)  NULL,
    [CarrierReference] NVARCHAR (255)   NULL,
    [Description]      NVARCHAR (1000)  NULL,
    [SaleZoneId]       BIGINT           NULL,
    [CustomerId]       INT              NULL,
    [StatusId]         UNIQUEIDENTIFIER NULL,
    [Notes]            NVARCHAR (1000)  NULL,
    [ReasonId]         UNIQUEIDENTIFIER NULL,
    [Attachments]      NVARCHAR (MAX)   NULL,
    [WorkGroupId]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_CustomerCase] PRIMARY KEY CLUSTERED ([ID] ASC)
);

