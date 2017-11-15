CREATE TABLE [TOneWhS_Sales].[RP_CustomerCountry_New] (
    [ID]                BIGINT   NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [CustomerID]        INT      NOT NULL,
    [CountryID]         INT      NOT NULL,
    [BED]               DATETIME NOT NULL,
    [EED]               DATETIME NULL
);




GO
CREATE CLUSTERED INDEX [IX_RP_CustomerCountry_New_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_CustomerCountry_New]([ProcessInstanceID] ASC);

