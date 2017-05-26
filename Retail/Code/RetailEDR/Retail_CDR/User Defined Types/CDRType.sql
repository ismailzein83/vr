CREATE TYPE [Retail_CDR].[CDRType] AS TABLE (
    [ID]                   BIGINT          NULL,
    [Call_Id]              VARCHAR (100)   NULL,
    [AttemptDateTime]      DATETIME        NULL,
    [ConnectDateTime]      DATETIME        NULL,
    [DisconnectDateTime]   DATETIME        NULL,
    [DurationInSeconds]    DECIMAL (20, 4) NULL,
    [DisconnectReason]     VARCHAR (100)   NULL,
    [CallProgressState]    VARCHAR (100)   NULL,
    [Account]              VARCHAR (100)   NULL,
    [OriginatorId]         VARCHAR (500)   NULL,
    [OriginatorNumber]     VARCHAR (100)   NULL,
    [OriginatorFromNumber] VARCHAR (100)   NULL,
    [OriginalDialedNumber] VARCHAR (100)   NULL,
    [TerminatorId]         VARCHAR (500)   NULL,
    [TerminatorNumber]     VARCHAR (100)   NULL,
    [IncomingGwId]         VARCHAR (100)   NULL,
    [OutgoingGwId]         VARCHAR (100)   NULL,
    [TransferredCall_Id]   VARCHAR (100)   NULL,
    [OriginatorIP]         VARCHAR (50)    NULL,
    [TerminatorIP]         VARCHAR (50)    NULL,
    [InitiationCallType]   INT             NULL,
    [TerminationCallType]  INT             NULL,
    [FileName]             VARCHAR (200)   NULL,
    [OriginatorExtension]  VARCHAR (20)    NULL,
    [TerminatorExtension]  VARCHAR (20)    NULL,
    [QueueItemId]          BIGINT          NULL);













