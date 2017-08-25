CREATE TABLE [Retail_CDR].[CDR] (
    [ID]                   BIGINT           NOT NULL,
    [Call_Id]              VARCHAR (200)    NULL,
    [DataSourceId]         UNIQUEIDENTIFIER NULL,
    [AttemptDateTime]      DATETIME         NULL,
    [ConnectDateTime]      DATETIME         NULL,
    [DisconnectDateTime]   DATETIME         NULL,
    [DurationInSeconds]    DECIMAL (20, 4)  NULL,
    [DisconnectReason]     VARCHAR (100)    NULL,
    [CallProgressState]    VARCHAR (100)    NULL,
    [Account]              VARCHAR (100)    NULL,
    [OriginatorId]         VARCHAR (500)    NULL,
    [OriginatorNumber]     VARCHAR (100)    NULL,
    [OriginatorFromNumber] VARCHAR (100)    NULL,
    [OriginalDialedNumber] VARCHAR (100)    NULL,
    [TerminatorId]         VARCHAR (500)    NULL,
    [TerminatorNumber]     VARCHAR (100)    NULL,
    [IncomingGwId]         VARCHAR (100)    NULL,
    [OutgoingGwId]         VARCHAR (100)    NULL,
    [TransferredCall_Id]   VARCHAR (100)    NULL,
    [OriginatorIP]         VARCHAR (250)    NULL,
    [TerminatorIP]         VARCHAR (250)    NULL,
    [InitiationCallType]   INT              NULL,
    [TerminationCallType]  INT              NULL,
    [FileName]             VARCHAR (200)    NULL,
    [QueueItemId]          BIGINT           NULL,
    [OriginatorExtension]  VARCHAR (20)     NULL,
    [TerminatorExtension]  VARCHAR (20)     NULL,
    CONSTRAINT [IX_CDR_CDRId] UNIQUE NONCLUSTERED ([ID] ASC)
);
























GO



GO
CREATE CLUSTERED INDEX [IX_CDR_AttemptDateTime]
    ON [Retail_CDR].[CDR]([AttemptDateTime] ASC);

