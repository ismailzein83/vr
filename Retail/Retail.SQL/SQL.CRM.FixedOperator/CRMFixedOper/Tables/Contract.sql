CREATE TABLE [CRMFixedOper].[Contract] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [ResourceName]      NVARCHAR (255)   NULL,
    [BillingContractId] NVARCHAR (255)   NULL,
    [CustomerID]        BIGINT           NULL,
    [TypeID]            UNIQUEIDENTIFIER NULL,
    [StatusID]          UNIQUEIDENTIFIER NULL,
    [OrderID]           BIGINT           NULL,
    [CreatedTime]       DATETIME         NULL,
    [CreatedBy]         INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [LastModifiedBy]    INT              NULL,
    CONSTRAINT [PK__Contract__3214EC271A14E395] PRIMARY KEY CLUSTERED ([ID] ASC)
);

