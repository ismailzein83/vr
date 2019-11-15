CREATE TABLE [RetailBilling].[ContractServiceTypeOption] (
    [ID]                    UNIQUEIDENTIFIER NOT NULL,
    [Name]                  NVARCHAR (255)   NULL,
    [ContractServiceTypeID] UNIQUEIDENTIFIER NULL,
    [CreatedTime]           DATETIME         NULL,
    [CreatedBy]             INT              NULL,
    [LastModifiedTime]      DATETIME         NULL,
    [LastModifiedBy]        INT              NULL,
    [timestamp]             ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

