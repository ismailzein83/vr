CREATE TABLE [TOneWhS_SPL].[SupplierPriceListTemplate] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [SupplierID]    INT            NULL,
    [ConfigDetails] NVARCHAR (MAX) NULL,
    [Draft]         NVARCHAR (MAX) NULL,
    [CreatedTime]   DATETIME       CONSTRAINT [DF_PriceListTemplate_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]     ROWVERSION     NULL,
    CONSTRAINT [PK_PriceListTemplate] PRIMARY KEY CLUSTERED ([ID] ASC)
);

