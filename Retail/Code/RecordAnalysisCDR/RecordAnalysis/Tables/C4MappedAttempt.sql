CREATE TABLE [RecordAnalysis].[C4MappedAttempt] (
    [AttemptId]                 BIGINT          NULL,
    [AttemptDateTime]           DATETIME        NULL,
    [InInterconnection]         INT             NULL,
    [OutInterconnection]        INT             NULL,
    [ProbeId]                   INT             NULL,
    [CDPN]                      VARCHAR (50)    NULL,
    [OrigCDPN]                  VARCHAR (50)    NULL,
    [CGPN]                      VARCHAR (50)    NULL,
    [OrigCGPN]                  VARCHAR (50)    NULL,
    [InIP]                      VARCHAR (50)    NULL,
    [OutIP]                     VARCHAR (50)    NULL,
    [OriginationSaleZone]       BIGINT          NULL,
    [OriginationSaleCurrencyId] INT             NULL,
    [OriginationSaleRate]       DECIMAL (20, 4) NULL,
    [SaleZone]                  BIGINT          NULL,
    [SaleCurrencyId]            INT             NULL,
    [SaleRate]                  DECIMAL (20, 4) NULL,
    [QueueItemId]               BIGINT          NULL
);


GO
CREATE CLUSTERED INDEX [IX_C4MappedAttempt_AttemptDateTime]
    ON [RecordAnalysis].[C4MappedAttempt]([AttemptDateTime] ASC);

