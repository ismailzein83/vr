CREATE TABLE [TOneWhS_Deal].[DealSaleZone] (
    [ID]                  INT             IDENTITY (1, 1) NOT NULL,
    [DealSaleZoneGroupID] INT             NOT NULL,
    [ZoneID]              BIGINT          NOT NULL,
    [Rate]                DECIMAL (20, 8) NOT NULL,
    [RateTypeID]          INT             NULL,
    [CreatedTime]         DATETIME        CONSTRAINT [DF_DealSaleZone_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]           ROWVERSION      NULL,
    CONSTRAINT [PK_DealSaleZone] PRIMARY KEY CLUSTERED ([ID] ASC)
);

