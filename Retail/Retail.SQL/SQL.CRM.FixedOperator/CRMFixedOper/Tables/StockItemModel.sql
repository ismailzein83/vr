CREATE TABLE [CRMFixedOper].[StockItemModel] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Type]             UNIQUEIDENTIFIER NULL,
    [Image]            NVARCHAR (255)   NULL,
    [Price]            DECIMAL (20, 4)  NULL,
    [Description]      NVARCHAR (255)   NULL,
    [Vender]           UNIQUEIDENTIFIER NULL,
    [OriginCountry]    INT              NULL,
    [ProductionDate]   DATETIME         NULL,
    [Weight]           DECIMAL (20, 4)  NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [ModelCode]        NVARCHAR (255)   NULL,
    [timestamp]        ROWVERSION       NULL,
    [NumberOfPorts]    INT              NULL,
    [SupportWIFI]      BIT              NULL,
    [SpeedId]          UNIQUEIDENTIFIER NULL,
    [ConnectionTypeId] UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);





