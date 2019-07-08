CREATE TABLE [CRMFixedOper].[Payment] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [POSID]            UNIQUEIDENTIFIER NULL,
    [CustomerID]       BIGINT           NULL,
    [BillingAccountID] BIGINT           NULL,
    [Amount]           DECIMAL (24, 6)  NULL,
    [CurrencyID]       INT              NULL,
    [PaymentType]      INT              NULL,
    [BankID]           INT              NULL,
    [CheckNumber]      NVARCHAR (255)   NULL,
    [CheckDate]        DATETIME         NULL,
    [OwnerName]        NVARCHAR (255)   NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [ReceivedBy]       INT              NULL,
    [ReceivedTime]     DATETIME         NULL,
    [Printed]          BIT              NULL,
    CONSTRAINT [PK__Payment__3214EC27756D6ECB] PRIMARY KEY CLUSTERED ([ID] ASC)
);

