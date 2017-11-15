CREATE TABLE [TOneWhS_BE].[CP_SaleZoneRoutingProducts_Changed] (
    [ID]                BIGINT   NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [EED]               DATETIME NULL
);




GO
CREATE CLUSTERED INDEX [IX_CP_SaleZoneRoutingProducts_Changed_ProcessInstanceID]
    ON [TOneWhS_BE].[CP_SaleZoneRoutingProducts_Changed]([ProcessInstanceID] ASC);

