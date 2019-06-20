CREATE TABLE [RetailBilling].[ContractType] (
    [ID]                   UNIQUEIDENTIFIER NOT NULL,
    [Name]                 NVARCHAR (255)   NULL,
    [TechnicalServiceType] INT              NULL,
    [CreatedTime]          DATETIME         NULL,
    [CreatedBy]            INT              NULL,
    [LastModifiedTime]     DATETIME         NULL,
    [LastModifiedBy]       INT              NULL,
    [timestamp]            ROWVERSION       NULL,
    CONSTRAINT [PK__Contract__3214EC277F60ED59] PRIMARY KEY CLUSTERED ([ID] ASC)
);

