CREATE TABLE [RecordAnalysis].[C4InvalidAttempt] (
    [AttemptId]                 BIGINT          NOT NULL,
    [AttemptDateTime]           DATETIME        NULL,
    [InInterconnection]         INT             NULL,
    [OutInterconnection]        INT             NULL,
    [ProbeId]                   INT             NULL,
    [CDPN]                      NVARCHAR (255)  NULL,
    [OrigCDPN]                  NVARCHAR (255)  NULL,
    [CGPN]                      NVARCHAR (255)  NULL,
    [OrigCGPN]                  NVARCHAR (255)  NULL,
    [InIP]                      NVARCHAR (255)  NULL,
    [OutIP]                     NVARCHAR (255)  NULL,
    [OriginationSaleZone]       BIGINT          NULL,
    [OriginationSaleCurrencyId] INT             NULL,
    [OriginationSaleRate]       DECIMAL (20, 4) NULL,
    [SaleZone]                  BIGINT          NULL,
    [SaleCurrencyId]            INT             NULL,
    [SaleRate]                  DECIMAL (20, 4) NULL,
    [QueueItemId]               BIGINT          NULL
);


GO
CREATE CLUSTERED INDEX [IX_C4InvalidAttempt_AttemptDateTime]
    ON [RecordAnalysis].[C4InvalidAttempt]([AttemptDateTime] ASC);

