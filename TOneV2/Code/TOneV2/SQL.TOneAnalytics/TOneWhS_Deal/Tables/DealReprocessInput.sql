CREATE TABLE [TOneWhS_Deal].[DealReprocessInput] (
    [ID]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [DealID]            INT             NOT NULL,
    [ZoneGroupNb]       INT             NOT NULL,
    [IsSale]            BIT             NOT NULL,
    [FromTime]          DATETIME        NOT NULL,
    [ToTime]            DATETIME        NOT NULL,
    [UpToDurationInSec] DECIMAL (20, 4) NOT NULL,
    [TierNb]            INT             NOT NULL,
    [RateTierNb]        INT             NOT NULL,
    [CreatedTime]       DATETIME        CONSTRAINT [DF_DealReprocessInput_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_DealReprocessInput] PRIMARY KEY CLUSTERED ([ID] ASC)
);

