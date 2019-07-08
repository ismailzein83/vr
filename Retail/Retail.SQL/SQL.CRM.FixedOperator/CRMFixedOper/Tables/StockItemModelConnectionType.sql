CREATE TABLE [CRMFixedOper].[StockItemModelConnectionType] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_StockItemModelConnectionType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

