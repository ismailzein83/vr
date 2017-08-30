CREATE TABLE [Retail_CDR].[CDR] (
    [ID]                   BIGINT           NOT NULL,
    [Call_Id]              VARCHAR (500)    NULL,
    [DataSourceId]         UNIQUEIDENTIFIER NULL,
    [AttemptDateTime]      DATETIME         NULL,
    [ConnectDateTime]      DATETIME         NULL,
    [DisconnectDateTime]   DATETIME         NULL,
    [DurationInSeconds]    DECIMAL (20, 4)  NULL,
    [DisconnectReason]     VARCHAR (200)    NULL,
    [CallProgressState]    VARCHAR (200)    NULL,
    [Account]              VARCHAR (200)    NULL,
    [OriginatorId]         VARCHAR (500)    NULL,
    [OriginatorNumber]     VARCHAR (500)    NULL,
    [OriginatorFromNumber] VARCHAR (500)    NULL,
    [OriginalDialedNumber] VARCHAR (500)    NULL,
    [TerminatorId]         VARCHAR (500)    NULL,
    [TerminatorNumber]     VARCHAR (500)    NULL,
    [IncomingGwId]         VARCHAR (500)    NULL,
    [OutgoingGwId]         VARCHAR (500)    NULL,
    [TransferredCall_Id]   VARCHAR (500)    NULL,
    [OriginatorIP]         VARCHAR (250)    NULL,
    [TerminatorIP]         VARCHAR (250)    NULL,
    [InitiationCallType]   INT              NULL,
    [TerminationCallType]  INT              NULL,
    [FileName]             VARCHAR (255)    NULL,
    [QueueItemId]          BIGINT           NULL,
    [OriginatorExtension]  VARCHAR (500)    NULL,
    [TerminatorExtension]  VARCHAR (500)    NULL,
    CONSTRAINT [IX_CDR_CDRId] UNIQUE NONCLUSTERED ([ID] ASC)
);


























GO



GO
CREATE CLUSTERED INDEX [IX_CDR_AttemptDateTime]
    ON [Retail_CDR].[CDR]([AttemptDateTime] ASC);

