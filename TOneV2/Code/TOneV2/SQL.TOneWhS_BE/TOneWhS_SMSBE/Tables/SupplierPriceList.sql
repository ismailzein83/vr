CREATE TABLE [TOneWhS_SMSBE].[SupplierPriceList] (
    [ID]                INT        NOT NULL,
    [SupplierID]        INT        NOT NULL,
    [CurrencyID]        INT        NOT NULL,
    [EffectiveOn]       DATETIME   NOT NULL,
    [ProcessInstanceID] BIGINT     NOT NULL,
    [UserID]            INT        NOT NULL,
    [CreatedTime]       DATETIME   CONSTRAINT [DF_SupplierSMSPriceList_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [LastModifiedTime]  DATETIME   CONSTRAINT [DF_SupplierSMSPriceList_LastModifiedTime] DEFAULT (getdate()) NOT NULL,
    [timestamp]         ROWVERSION NULL,
    CONSTRAINT [PK_SupplierSMSPriceList] PRIMARY KEY CLUSTERED ([ID] ASC)
);

