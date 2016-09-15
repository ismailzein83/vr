CREATE TABLE [TOneWhS_Sales].[RP_SaleZoneService_Preview] (
    [ZoneName]                  NVARCHAR (255) NOT NULL,
    [ProcessInstanceID]         BIGINT         NOT NULL,
    [CurrentServices]           NVARCHAR (MAX) NULL,
    [IsCurrentServiceInherited] BIT            NULL,
    [NewServices]               NVARCHAR (MAX) NULL,
    [EffectiveOn]               DATETIME       NOT NULL,
    [EffectiveUntil]            DATETIME       NULL
);

