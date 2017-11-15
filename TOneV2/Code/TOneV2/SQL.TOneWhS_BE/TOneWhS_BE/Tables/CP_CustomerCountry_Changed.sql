CREATE TABLE [TOneWhS_BE].[CP_CustomerCountry_Changed] (
    [ID]                BIGINT   NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [EED]               DATETIME NULL
);




GO
CREATE CLUSTERED INDEX [IX_CP_CustomerCountry_Changed_ProcessInstanceID]
    ON [TOneWhS_BE].[CP_CustomerCountry_Changed]([ProcessInstanceID] ASC);

