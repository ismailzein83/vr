CREATE TABLE [TOneWhS_Deal].[DealDetailedProgress] (
    [ID]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [DealID]               INT             NOT NULL,
    [ZoneGroupNb]          INT             NOT NULL,
    [IsSale]               BIT             NOT NULL,
    [TierNb]               INT             NULL,
    [RateTierNb]           INT             NULL,
    [FromTime]             DATETIME        NOT NULL,
    [ToTime]               DATETIME        NOT NULL,
    [ReachedDurationInSec] DECIMAL (20, 4) NOT NULL,
    [CreatedTime]          DATETIME        CONSTRAINT [DF_DealDetailedProgress_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]            ROWVERSION      NULL,
    CONSTRAINT [PK_DealDetailedProgress_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);



