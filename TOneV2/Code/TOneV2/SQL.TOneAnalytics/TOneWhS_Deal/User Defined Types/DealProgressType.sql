CREATE TYPE [TOneWhS_Deal].[DealProgressType] AS TABLE (
    [DealProgressID]       BIGINT          NULL,
    [DealID]               INT             NOT NULL,
    [ZoneGroupNb]          INT             NOT NULL,
    [IsSale]               BIT             NOT NULL,
    [CurrentTierNb]        INT             NOT NULL,
    [ReachedDurationInSec] DECIMAL (20, 4) NULL,
    [TargetDurationInSec]  DECIMAL (20, 4) NULL);

