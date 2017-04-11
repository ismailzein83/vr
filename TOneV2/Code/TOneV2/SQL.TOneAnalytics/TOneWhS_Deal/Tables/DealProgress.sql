CREATE TABLE [TOneWhS_Deal].[DealProgress] (
    [ID]                   BIGINT           IDENTITY (1, 1) NOT NULL,
    [ZoneGroupId]          UNIQUEIDENTIFIER NOT NULL,
    [IsSale]               BIT              NOT NULL,
    [TierNb]               INT              NOT NULL,
    [RateTierNb]           INT              NOT NULL,
    [FromTime]             DATETIME         NOT NULL,
    [ToTime]               DATETIME         NOT NULL,
    [ReachedDurationInSec] DECIMAL (20, 4)  NOT NULL,
    [CreatedTime]          DATETIME         CONSTRAINT [DF_DealProgress_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_DealSaleProgress_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);

