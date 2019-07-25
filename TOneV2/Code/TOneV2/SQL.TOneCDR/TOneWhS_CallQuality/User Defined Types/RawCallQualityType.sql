CREATE TYPE [TOneWhS_CallQuality].[RawCallQualityType] AS TABLE (
    [Id]              BIGINT        NULL,
    [IDonSwitch]      BIGINT        NULL,
    [AttemptDateTime] DATETIME      NULL,
    [CGPN]            VARCHAR (40)  NULL,
    [CDPN]            VARCHAR (40)  NULL,
    [OutCarrier]      VARCHAR (100) NULL,
    [IsCLI]           BIT           NULL,
    [IsFAS]           BIT           NULL,
    [QueueItemId]     BIGINT        NULL);

