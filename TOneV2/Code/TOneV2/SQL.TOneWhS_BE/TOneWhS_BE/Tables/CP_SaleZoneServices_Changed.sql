CREATE TABLE [TOneWhS_BE].[CP_SaleZoneServices_Changed] (
    [ID]                BIGINT   NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [EED]               DATETIME NULL
);




GO
CREATE CLUSTERED INDEX [IX_CP_SaleZoneServices_Changed_ProcessInstanceID]
    ON [TOneWhS_BE].[CP_SaleZoneServices_Changed]([ProcessInstanceID] ASC);

