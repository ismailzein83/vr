CREATE TABLE [RetailBilling].[ContractServiceChargeableCondition] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    [Condition]        NVARCHAR (MAX)   NOT NULL,
    [Priority]         INT              NULL,
    CONSTRAINT [PK__Contract__3214EC27540C7B00] PRIMARY KEY CLUSTERED ([ID] ASC)
);

