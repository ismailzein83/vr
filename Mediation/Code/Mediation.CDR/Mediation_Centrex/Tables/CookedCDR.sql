CREATE TABLE [Mediation_Centrex].[CookedCDR] (
    [CallId]               NVARCHAR (200)  NULL,
    [ConnectDateTime]      DATETIME        NULL,
    [DisconnectDateTime]   DATETIME        NULL,
    [DisconnectReason]     VARCHAR (200)   NULL,
    [CallProgressState]    VARCHAR (200)   NULL,
    [Account]              VARCHAR (200)   NULL,
    [OriginatorId]         VARCHAR (200)   NULL,
    [OriginatorNumber]     VARCHAR (200)   NULL,
    [OriginatorFromNumber] VARCHAR (200)   NULL,
    [OriginalDialedNumber] VARCHAR (200)   NULL,
    [TerminatorId]         VARCHAR (200)   NULL,
    [TerminatorNumber]     VARCHAR (200)   NULL,
    [IncomingGwId]         VARCHAR (200)   NULL,
    [OutgoingGwId]         VARCHAR (200)   NULL,
    [TransferredCallId]    VARCHAR (200)   NULL,
    [DurationInSeconds]    DECIMAL (10, 4) NULL,
    [AttemptDateTime]      DATETIME        NULL,
    [SendCallType]         TINYINT         NULL,
    [ReveiveCallType]      TINYINT         NULL,
    [FileName]             NVARCHAR (255)  NULL,
    [ReplacedCallId]       NVARCHAR (255)  NULL,
    [OriginatorExtension]  VARCHAR (10)    NULL,
    [TerminatorExtension]  VARCHAR (10)    NULL
);



