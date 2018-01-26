CREATE TABLE [TOneWhS_Sales].[RP_CustomerCountry_ChangedPreview] (
    [ID]                INT      NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [EED]               DATETIME NOT NULL,
    [CustomerID]        INT      NULL
);






GO
CREATE CLUSTERED INDEX [IX_RP_CustomerCountry_ChangedPreview_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_CustomerCountry_ChangedPreview]([ProcessInstanceID] ASC);

