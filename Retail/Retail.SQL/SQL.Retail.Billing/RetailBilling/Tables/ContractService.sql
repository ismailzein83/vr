CREATE TABLE [RetailBilling].[ContractService] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [ContractID]       BIGINT           NULL,
    [ServiceID]        UNIQUEIDENTIFIER NULL,
    [StatusID]         UNIQUEIDENTIFIER NULL,
    [BillingAccountID] BIGINT           NULL,
    [ActivationDate]   DATETIME         NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [SuspensionDate]   DATETIME         NULL,
    CONSTRAINT [PK__Contract__3214EC270B91BA14] PRIMARY KEY CLUSTERED ([ID] ASC)
);

