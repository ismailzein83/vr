CREATE TABLE [RecordAnalysis].[C4BadCDRs] (
    [CDRId]               BIGINT          NOT NULL,
    [AttemptDateTime]     DATETIME        NULL,
    [AlertDateTime]       DATETIME        NULL,
    [ConnectDateTime]     DATETIME        NULL,
    [DisconnectDateTime]  DATETIME        NULL,
    [InInterconnection]   INT             NULL,
    [OutInterconnection]  INT             NULL,
    [DurationInSeconds]   DECIMAL (20, 4) NULL,
    [Switch]              INT             NULL,
    [CDPN]                NVARCHAR (255)  NULL,
    [OrigCDPN]            NVARCHAR (255)  NULL,
    [CGPN]                NVARCHAR (255)  NULL,
    [OrigCGPN]            NVARCHAR (255)  NULL,
    [InTrunk]             NVARCHAR (255)  NULL,
    [OutTrunk]            NVARCHAR (255)  NULL,
    [OriginationSaleZone] BIGINT          NULL,
    [SaleZone]            BIGINT          NULL,
    [IsDelivered]         BIT             NULL,
    [IsRerouted]          BIT             NULL,
    [PDDInSeconds]        DECIMAL (20, 4) NULL,
    [QueueItemId]         BIGINT          NULL
);




GO
CREATE CLUSTERED INDEX [IX_C4BadCDRs_AttemptDateTime]
    ON [RecordAnalysis].[C4BadCDRs]([AttemptDateTime] ASC);

