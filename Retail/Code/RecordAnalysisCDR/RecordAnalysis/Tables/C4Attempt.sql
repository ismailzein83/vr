CREATE TABLE [RecordAnalysis].[C4Attempt] (
    [Id]                   BIGINT       NOT NULL,
    [AttemptDateTime]      DATETIME     NULL,
    [CDPN]                 VARCHAR (50) NULL,
    [CGPN]                 VARCHAR (50) NULL,
    [ProbeId]              INT          NULL,
    [InIP]                 VARCHAR (50) NULL,
    [OutIP]                VARCHAR (50) NULL,
    [QueueItemId]          BIGINT       NULL,
    [OriginationPointCode] VARCHAR (50) NULL,
    [DestinationPointCode] VARCHAR (50) NULL
);


GO
CREATE CLUSTERED INDEX [IX_C4Attempt_AttemptDateTime]
    ON [RecordAnalysis].[C4Attempt]([AttemptDateTime] ASC);

