CREATE TABLE [RecordAnalysis].[C4InvalidCDRs] (
    [CDRId]                     BIGINT          NOT NULL,
    [AttemptDateTime]           DATETIME        NULL,
    [AlertDateTime]             DATETIME        NULL,
    [ConnectDateTime]           DATETIME        NULL,
    [DisconnectDateTime]        DATETIME        NULL,
    [InInterconnection]         INT             NULL,
    [OutInterconnection]        INT             NULL,
    [DurationInSeconds]         DECIMAL (20, 4) NULL,
    [Switch]                    INT             NULL,
    [CDPN]                      NVARCHAR (255)  NULL,
    [OrigCDPN]                  NVARCHAR (255)  NULL,
    [CGPN]                      NVARCHAR (255)  NULL,
    [OrigCGPN]                  NVARCHAR (255)  NULL,
    [InTrunk]                   NVARCHAR (255)  NULL,
    [OutTrunk]                  NVARCHAR (255)  NULL,
    [OriginationSaleZone]       BIGINT          NULL,
    [OriginationSaleCurrencyId] INT             NULL,
    [OriginationSaleRate]       DECIMAL (20, 4) NULL,
    [SaleZone]                  BIGINT          NULL,
    [SaleDurationInSeconds]     DECIMAL (20, 4) NULL,
    [SaleCurrencyId]            INT             NULL,
    [SaleRate]                  DECIMAL (20, 4) NULL,
    [SaleNet]                   DECIMAL (20, 4) NULL,
    [IsDelivered]               BIT             NULL,
    [IsRerouted]                BIT             NULL,
    [PDDInSeconds]              DECIMAL (20, 4) NULL,
    [QueueItemId]               BIGINT          NULL
);


GO
CREATE CLUSTERED INDEX [IX_C4InvalidCDRs_AttemptDateTime]
    ON [RecordAnalysis].[C4InvalidCDRs]([AttemptDateTime] ASC);

