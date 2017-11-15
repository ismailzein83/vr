CREATE TABLE [TOneWhS_Sales].[RP_SaleZoneService_Changed] (
    [ID]                BIGINT   NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [EED]               DATETIME NULL
);




GO
CREATE CLUSTERED INDEX [IX_RP_SaleZoneService_Changed_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_SaleZoneService_Changed]([ProcessInstanceID] ASC);

