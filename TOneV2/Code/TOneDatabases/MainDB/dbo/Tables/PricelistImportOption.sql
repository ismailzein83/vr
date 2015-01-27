CREATE TABLE [dbo].[PricelistImportOption] (
    [SupplierID]       VARCHAR (10)  NOT NULL,
    [ImporterName]     VARCHAR (255) NULL,
    [ImportParameters] NTEXT         NULL,
    [LastUpdate]       SMALLDATETIME CONSTRAINT [DF_PricelistImportOption_LastUpdate] DEFAULT (getdate()) NULL,
    [UserID]           INT           NULL,
    [timestamp]        ROWVERSION    NULL,
    CONSTRAINT [PK_PricelistImportOption] PRIMARY KEY CLUSTERED ([SupplierID] ASC)
);

