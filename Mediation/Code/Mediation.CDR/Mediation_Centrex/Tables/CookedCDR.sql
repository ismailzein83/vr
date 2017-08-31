CREATE TABLE [Mediation_Centrex].[CookedCDR] (
    [CDRID]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [CallId]               NVARCHAR (500)  NULL,
    [ConnectDateTime]      DATETIME        NULL,
    [DisconnectDateTime]   DATETIME        NULL,
    [DisconnectReason]     VARCHAR (200)   NULL,
    [CallProgressState]    VARCHAR (200)   NULL,
    [Account]              VARCHAR (200)   NULL,
    [OriginatorId]         VARCHAR (500)   NULL,
    [OriginatorNumber]     VARCHAR (500)   NULL,
    [OriginatorFromNumber] VARCHAR (500)   NULL,
    [OriginalDialedNumber] VARCHAR (500)   NULL,
    [TerminatorId]         VARCHAR (500)   NULL,
    [TerminatorNumber]     VARCHAR (500)   NULL,
    [IncomingGwId]         VARCHAR (200)   NULL,
    [OutgoingGwId]         VARCHAR (200)   NULL,
    [TransferredCallId]    VARCHAR (200)   NULL,
    [DurationInSeconds]    DECIMAL (20, 4) NULL,
    [AttemptDateTime]      DATETIME        NULL,
    [SendCallType]         TINYINT         NULL,
    [ReveiveCallType]      TINYINT         NULL,
    [FileName]             NVARCHAR (255)  NULL,
    [ReplacedCallId]       NVARCHAR (255)  NULL,
    [OriginatorExtension]  VARCHAR (20)    NULL,
    [TerminatorExtension]  VARCHAR (20)    NULL,
    [OriginatorIp]         VARCHAR (100)   NULL,
    [TerminatorIp]         VARCHAR (100)   NULL,
    CONSTRAINT [PK_CookedCDR] PRIMARY KEY CLUSTERED ([CDRID] ASC)
);









