CREATE TABLE [TOneWhS_Sales].[RP_CustomerCountry_NewPreview] (
    [ID]                INT      NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [BED]               DATETIME NOT NULL,
    [EED]               DATETIME NULL,
    [CustomerID]        INT      NULL
);






GO
CREATE CLUSTERED INDEX [IX_RP_CustomerCountry_NewPreview_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_CustomerCountry_NewPreview]([ProcessInstanceID] ASC);

