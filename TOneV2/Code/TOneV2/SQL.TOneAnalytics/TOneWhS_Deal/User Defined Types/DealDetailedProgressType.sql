CREATE TYPE [TOneWhS_Deal].[DealDetailedProgressType] AS TABLE (
    [DealDetailedProgressID] BIGINT          NULL,
    [DealID]                 INT             NOT NULL,
    [ZoneGroupNb]            INT             NOT NULL,
    [IsSale]                 BIT             NOT NULL,
    [TierNb]                 INT             NULL,
    [RateTierNb]             INT             NULL,
    [ReachedDurationInSec]   DECIMAL (20, 4) NOT NULL,
    [FromTime]               DATETIME        NULL,
    [ToTime]                 DATETIME        NULL);

