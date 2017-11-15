CREATE TABLE [TOneWhS_Sales].[RP_SaleRate_Preview] (
    [ZoneName]               NVARCHAR (255)  NOT NULL,
    [ProcessInstanceID]      BIGINT          NOT NULL,
    [RateTypeID]             INT             NULL,
    [CurrentRate]            DECIMAL (20, 8) NULL,
    [IsCurrentRateInherited] BIT             NULL,
    [NewRate]                DECIMAL (20, 8) NULL,
    [ChangeType]             INT             NOT NULL,
    [EffectiveOn]            DATETIME        NULL,
    [EffectiveUntil]         DATETIME        NULL,
    [CurrencyId]             INT             NOT NULL
);














GO
CREATE NONCLUSTERED INDEX [IX_RP_SaleRate_Preview_ZoneName]
    ON [TOneWhS_Sales].[RP_SaleRate_Preview]([ZoneName] ASC);


GO
CREATE CLUSTERED INDEX [IX_RP_SaleRate_Preview_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_SaleRate_Preview]([ProcessInstanceID] ASC);

