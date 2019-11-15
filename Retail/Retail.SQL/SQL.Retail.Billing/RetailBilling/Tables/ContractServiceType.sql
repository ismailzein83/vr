CREATE TABLE [RetailBilling].[ContractServiceType] (
    [ID]                UNIQUEIDENTIFIER NOT NULL,
    [Name]              NVARCHAR (255)   NULL,
    [ContractTypeID]    UNIQUEIDENTIFIER NULL,
    [ServiceCategoryID] UNIQUEIDENTIFIER NULL,
    [CreatedTime]       DATETIME         NULL,
    [CreatedBy]         INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [LastModifiedBy]    INT              NULL,
    [timestamp]         ROWVERSION       NULL,
    CONSTRAINT [PK__Contract__3214EC274C6B5938] PRIMARY KEY CLUSTERED ([ID] ASC)
);

