CREATE TABLE [Mediation_Generic].[CookedCDR] (
    [CDRID]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [CallId]               NVARCHAR (100)  NULL,
    [ConnectDateTime]      DATETIME        NULL,
    [DisconnectDateTime]   DATETIME        NULL,
    [DisconnectReason]     NVARCHAR (100)  NULL,
    [CallProgressState]    NVARCHAR (100)  NULL,
    [Account]              NVARCHAR (100)  NULL,
    [OriginatorId]         NVARCHAR (500)  NULL,
    [OriginatorNumber]     NVARCHAR (500)  NULL,
    [OriginatorFromNumber] NVARCHAR (500)  NULL,
    [OriginalDialedNumber] NVARCHAR (500)  NULL,
    [TerminatorId]         NVARCHAR (500)  NULL,
    [TerminatorNumber]     NVARCHAR (500)  NULL,
    [IncomingGwId]         NVARCHAR (500)  NULL,
    [OutgoingGwId]         NVARCHAR (500)  NULL,
    [TransferredCallId]    NVARCHAR (500)  NULL,
    [DurationInSeconds]    DECIMAL (10, 4) NULL,
    [OriginatorIp]         NVARCHAR (500)  NULL,
    [TerminatorIp]         NVARCHAR (500)  NULL,
    [AttemptDateTime]      DATETIME        NULL,
    [SendCallType]         TINYINT         NULL,
    [ReveiveCallType]      TINYINT         NULL,
    [FileName]             NVARCHAR (255)  NULL,
    [ReplacedCallId]       NVARCHAR (255)  NULL,
    CONSTRAINT [PK_CookedCDR] PRIMARY KEY CLUSTERED ([CDRID] ASC)
);













