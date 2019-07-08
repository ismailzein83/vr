CREATE TABLE [CRMFixedOper].[VPNLegType] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Speed]            INT              NULL,
    [ContractTypeID]   UNIQUEIDENTIFIER NULL,
    [BillingServiceID] UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    [Technology]       UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

