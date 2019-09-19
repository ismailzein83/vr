﻿CREATE TABLE [RecordAnalysis].[C4MappedCDR] (
    [CDRId]                     BIGINT          NULL,
    [AttemptDateTime]           DATETIME        NULL,
    [AlertDateTime]             DATETIME        NULL,
    [ConnectDateTime]           DATETIME        NULL,
    [DisconnectDateTime]        DATETIME        NULL,
    [InInterconnection]         INT             NULL,
    [OutInterconnection]        INT             NULL,
    [DurationInSeconds]         DECIMAL (20, 4) NULL,
    [Switch]                    INT             NULL,
    [CDPN]                      VARCHAR (50)    NULL,
    [OrigCDPN]                  VARCHAR (50)    NULL,
    [CGPN]                      VARCHAR (50)    NULL,
    [OrigCGPN]                  VARCHAR (50)    NULL,
    [InTrunk]                   VARCHAR (50)    NULL,
    [OutTrunk]                  VARCHAR (50)    NULL,
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
CREATE CLUSTERED INDEX [IX_C4MappedCDR_AttemptDateTime]
    ON [RecordAnalysis].[C4MappedCDR]([AttemptDateTime] ASC);

