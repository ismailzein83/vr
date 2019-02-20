CREATE TYPE [RecordAnalysis].[C4CDRType] AS TABLE (
    [Id]                   BIGINT          NULL,
    [AttemptDateTime]      DATETIME        NULL,
    [AlertDateTime]        DATETIME        NULL,
    [ConnectDateTime]      DATETIME        NULL,
    [DisconnectDateTime]   DATETIME        NULL,
    [DurationInSeconds]    DECIMAL (20, 4) NULL,
    [CDPN]                 VARCHAR (50)    NULL,
    [CGPN]                 VARCHAR (50)    NULL,
    [SwitchId]             INT             NULL,
    [InTrunk]              VARCHAR (50)    NULL,
    [OutTrunk]             VARCHAR (50)    NULL,
    [IsRerouted]           BIT             NULL,
    [CauseFromReleaseCode] VARCHAR (50)    NULL,
    [CauseToReleaseCode]   VARCHAR (50)    NULL,
    [QueueItemId]          BIGINT          NULL);

