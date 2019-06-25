CREATE TABLE [CRMFixedOper].[StockItem] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Barcode]          NVARCHAR (255)   NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [Customer]         BIGINT           NULL,
    [PurchaseDate]     DATETIME         NULL,
    [POS]              UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [Color]            UNIQUEIDENTIFIER NULL,
    [Model]            UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_StockItem] PRIMARY KEY CLUSTERED ([ID] ASC)
);

