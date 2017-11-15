CREATE TABLE [TOneWhS_BE].[CP_SaleCode_Changed] (
    [ID]                BIGINT   NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [EED]               DATETIME NULL
);




GO
CREATE CLUSTERED INDEX [IX_CP_SaleCode_Changed_ProcessInstanceID]
    ON [TOneWhS_BE].[CP_SaleCode_Changed]([ProcessInstanceID] ASC);

