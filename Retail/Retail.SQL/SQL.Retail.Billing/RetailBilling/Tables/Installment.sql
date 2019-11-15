CREATE TABLE [RetailBilling].[Installment] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [InstallmentTypeID] UNIQUEIDENTIFIER NULL,
    [BillingAccountID]  BIGINT           NULL,
    [ParentID]          VARCHAR (50)     NULL,
    [CurrencyID]        INT              NULL,
    [NbOfItems]         INT              NULL,
    [CreatedTime]       DATETIME         NULL,
    [CreatedBy]         INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [LastModifiedBy]    INT              NULL,
    CONSTRAINT [PK__Instalme__3214EC273493CFA7] PRIMARY KEY CLUSTERED ([ID] ASC)
);

