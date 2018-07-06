CREATE TYPE [ICX_Data].[AccumulatedDataType] AS TABLE (
    [Id]                   BIGINT           NULL,
    [UserSessionStartDate] DATETIME         NULL,
    [UserSession]          VARCHAR (110)    NULL,
    [UserName]             VARCHAR (50)     NULL,
    [SessionId]            VARCHAR (50)     NULL,
    [ISP]                  BIGINT           NULL,
    [LastReceivedIn]       DECIMAL (22, 2)  NULL,
    [TotalIn]              DECIMAL (22, 2)  NULL,
    [NumberOfInReset]      INT              NULL,
    [LastInTime]           DATETIME         NULL,
    [LastInResetTime]      DATETIME         NULL,
    [LastReceivedOut]      DECIMAL (22, 2)  NULL,
    [TotalOut]             DECIMAL (22, 2)  NULL,
    [NumberOfOutReset]     INT              NULL,
    [LastOutTime]          DATETIME         NULL,
    [LastOutResetTime]     DATETIME         NULL,
    [FirstBatchIdentifier] UNIQUEIDENTIFIER NULL);

