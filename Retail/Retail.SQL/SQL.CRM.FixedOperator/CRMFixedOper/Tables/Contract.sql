CREATE TABLE [CRMFixedOper].[Contract] (
    [ID]                       BIGINT           NOT NULL,
    [ResourceName]             NVARCHAR (255)   NULL,
    [BillingContractId]        NVARCHAR (255)   NULL,
    [CustomerID]               BIGINT           NULL,
    [TypeID]                   UNIQUEIDENTIFIER NULL,
    [StatusID]                 UNIQUEIDENTIFIER NULL,
    [OrderID]                  BIGINT           NULL,
    [CreatedTime]              DATETIME         NULL,
    [CreatedBy]                INT              NULL,
    [LastModifiedTime]         DATETIME         NULL,
    [LastModifiedBy]           INT              NULL,
    [RatePlanID]               INT              NULL,
    [CustomerBillingAccountID] BIGINT           NULL,
    [ActivationDate]           DATETIME         NULL,
    CONSTRAINT [PK__Contract__3214EC271A14E395] PRIMARY KEY CLUSTERED ([ID] ASC)
);



