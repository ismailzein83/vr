CREATE TABLE [TOneWhS_SMSBE].[SalePriceList] (
    [ID]                INT        NOT NULL,
    [CustomerID]        INT        NOT NULL,
    [CurrencyID]        INT        NOT NULL,
    [EffectiveOn]       DATETIME   NOT NULL,
    [ProcessInstanceID] BIGINT     NOT NULL,
    [UserID]            INT        NOT NULL,
    [CreatedTime]       DATETIME   CONSTRAINT [DF_CustomerSMSPriceList_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [LastModifiedTime]  DATETIME   CONSTRAINT [DF_CustomerSMSPriceList_LastModifiedTime] DEFAULT (getdate()) NOT NULL,
    [timestamp]         ROWVERSION NULL,
    CONSTRAINT [PK_CustomerSMSPriceList] PRIMARY KEY CLUSTERED ([ID] ASC)
);

