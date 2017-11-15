CREATE TABLE [TOneWhS_Sales].[RP_CustomerCountry_Changed] (
    [ID]                BIGINT   NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [EED]               DATETIME NOT NULL
);




GO
CREATE CLUSTERED INDEX [IX_RP_CustomerCountry_Changed_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_CustomerCountry_Changed]([ProcessInstanceID] ASC);

