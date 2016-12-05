CREATE TABLE [VR_Invoice].[BillingPeriodInfo] (
    [InvoiceTypeId]   UNIQUEIDENTIFIER NOT NULL,
    [PartnerId]       VARCHAR (50)     NOT NULL,
    [NextPeriodStart] DATETIME         NOT NULL,
    [CreatedTime]     DATETIME         CONSTRAINT [DF_BillingPeriodInfo_CreatedTime] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_BillingPeriodInfo] PRIMARY KEY CLUSTERED ([InvoiceTypeId] ASC, [PartnerId] ASC)
);

