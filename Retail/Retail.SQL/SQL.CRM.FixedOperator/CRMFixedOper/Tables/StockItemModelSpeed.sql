CREATE TABLE [CRMFixedOper].[StockItemModelSpeed] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Value]            DECIMAL (20, 4)  NULL,
    [SpeedUnit]        UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_StockItemSpeed] PRIMARY KEY CLUSTERED ([ID] ASC)
);

