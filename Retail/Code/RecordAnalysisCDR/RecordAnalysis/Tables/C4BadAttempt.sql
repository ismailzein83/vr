﻿CREATE TABLE [RecordAnalysis].[C4BadAttempt] (
    [AttemptId]           BIGINT         NOT NULL,
    [AttemptDateTime]     DATETIME       NULL,
    [InInterconnection]   INT            NULL,
    [OutInterconnection]  INT            NULL,
    [ProbeId]             INT            NULL,
    [CDPN]                NVARCHAR (255) NULL,
    [OrigCDPN]            NVARCHAR (255) NULL,
    [CGPN]                NVARCHAR (255) NULL,
    [OrigCGPN]            NVARCHAR (255) NULL,
    [InIP]                NVARCHAR (255) NULL,
    [OutIP]               NVARCHAR (255) NULL,
    [OriginationSaleZone] BIGINT         NULL,
    [SaleZone]            BIGINT         NULL,
    [QueueItemId]         BIGINT         NULL
);


GO
CREATE CLUSTERED INDEX [IX_C4BadAttempt_AttemptDateTime]
    ON [RecordAnalysis].[C4BadAttempt]([AttemptDateTime] ASC);

