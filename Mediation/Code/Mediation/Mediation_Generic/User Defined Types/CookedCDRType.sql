CREATE TYPE [Mediation_Generic].[CookedCDRType] AS TABLE (
    [CallId]               NVARCHAR (100)  NULL,
    [ConnectDateTime]      DATETIME        NULL,
    [DisconnectDateTime]   DATETIME        NULL,
    [DisconnectReason]     NVARCHAR (100)  NULL,
    [CallProgressState]    NVARCHAR (100)  NULL,
    [Account]              NVARCHAR (100)  NULL,
    [OriginatorId]         NVARCHAR (500)  NULL,
    [OriginatorNumber]     NVARCHAR (100)  NULL,
    [OriginatorFromNumber] NVARCHAR (100)  NULL,
    [OriginalDialedNumber] NVARCHAR (100)  NULL,
    [TerminatorId]         NVARCHAR (500)  NULL,
    [TerminatorNumber]     NVARCHAR (100)  NULL,
    [IncomingGwId]         NVARCHAR (100)  NULL,
    [OutgoingGwId]         NVARCHAR (100)  NULL,
    [TransferredCallId]    NVARCHAR (100)  NULL,
    [OriginatorIP]         NVARCHAR (50)   NULL,
    [TerminatorIP]         NVARCHAR (50)   NULL,
    [DurationInSeconds]    DECIMAL (10, 4) NULL);





