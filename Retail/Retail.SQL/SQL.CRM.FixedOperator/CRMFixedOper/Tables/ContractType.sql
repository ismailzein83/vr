CREATE TABLE [CRMFixedOper].[ContractType] (
    [ID]                    UNIQUEIDENTIFIER NOT NULL,
    [Name]                  NVARCHAR (255)   NULL,
    [BillingContractTypeID] UNIQUEIDENTIFIER NULL,
    [CreatedTime]           DATETIME         NULL,
    [CreatedBy]             INT              NULL,
    [LastModifiedTime]      DATETIME         NULL,
    [LastModifiedBy]        INT              NULL,
    [timestamp]             ROWVERSION       NULL,
    CONSTRAINT [PK__Contract__3214EC27164452B1] PRIMARY KEY CLUSTERED ([ID] ASC)
);



