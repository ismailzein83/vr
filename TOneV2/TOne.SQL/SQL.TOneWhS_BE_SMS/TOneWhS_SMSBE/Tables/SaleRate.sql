CREATE TABLE [TOneWhS_SMSBE].[SaleRate] (
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [PriceListID]      INT             NOT NULL,
    [MobileNetworkID]  INT             NOT NULL,
    [Rate]             DECIMAL (20, 8) NOT NULL,
    [BED]              DATETIME        NOT NULL,
    [EED]              DATETIME        NULL,
    [CreatedTime]      DATETIME        CONSTRAINT [DF_CustomerSMSSaleRate_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [LastModifiedTime] DATETIME        CONSTRAINT [DF_CustomerSMSSaleRate_LastModified] DEFAULT (getdate()) NOT NULL,
    [timestamp]        ROWVERSION      NULL,
    CONSTRAINT [PK_CustomerSMSSaleRate] PRIMARY KEY CLUSTERED ([ID] ASC)
);

