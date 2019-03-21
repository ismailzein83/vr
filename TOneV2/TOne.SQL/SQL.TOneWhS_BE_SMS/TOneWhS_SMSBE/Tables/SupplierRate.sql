CREATE TABLE [TOneWhS_SMSBE].[SupplierRate] (
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [PriceListID]      INT             NOT NULL,
    [MobileNetworkID]  INT             NOT NULL,
    [Rate]             DECIMAL (20, 8) NOT NULL,
    [BED]              DATETIME        NOT NULL,
    [EED]              DATETIME        NULL,
    [CreatedTime]      DATETIME        CONSTRAINT [DF_SMSSupplierRate_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [LastModifiedTime] DATETIME        CONSTRAINT [DF_SMSSupplierRate_LastModified] DEFAULT (getdate()) NOT NULL,
    [timestamp]        ROWVERSION      NULL,
    CONSTRAINT [PK_SMSSupplierRate] PRIMARY KEY CLUSTERED ([ID] ASC)
);

