CREATE TABLE [TOneWhS_Sales].[RP_SaleZoneService_New] (
    [ID]                BIGINT         NOT NULL,
    [ProcessInstanceID] BIGINT         NOT NULL,
    [Services]          NVARCHAR (MAX) NOT NULL,
    [ZoneID]            BIGINT         NOT NULL,
    [BED]               DATETIME       NOT NULL,
    [EED]               DATETIME       NULL
);




GO
CREATE CLUSTERED INDEX [IX_RP_SaleZoneService_New_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_SaleZoneService_New]([ProcessInstanceID] ASC);

