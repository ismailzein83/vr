CREATE TABLE [TOneWhS_Deal].[DealProgress] (
    [ID]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [DealID]               INT             NOT NULL,
    [ZoneGroupNb]          INT             NOT NULL,
    [IsSale]               BIT             NOT NULL,
    [CurrentTierNb]        INT             NOT NULL,
    [ReachedDurationInSec] DECIMAL (20, 4) NULL,
    [TargetDurationInSec]  DECIMAL (20, 4) NULL,
    [IsComplete]           AS              (case when [ReachedDurationInSec]=[TargetDurationInSec] then (1) else (0) end),
    [CreatedTime]          DATETIME        CONSTRAINT [DF_DealProgress_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [timestamp]            ROWVERSION      NULL,
    CONSTRAINT [PK_DealProgress] PRIMARY KEY CLUSTERED ([ID] ASC)
);





