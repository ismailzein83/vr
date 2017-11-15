CREATE TABLE [TOneWhS_BE].[CP_SaleZone_Changed] (
    [ID]                BIGINT   NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [EED]               DATETIME NULL
);




GO
CREATE CLUSTERED INDEX [IX_CP_SaleZone_Changed_ProcessInstanceID]
    ON [TOneWhS_BE].[CP_SaleZone_Changed]([ProcessInstanceID] ASC);

