CREATE TABLE [TOneWhs_CP].[SaleZone_Preview] (
    [ProcessInstanceID] BIGINT         NOT NULL,
    [CountryID]         INT            NOT NULL,
    [ZoneName]          NVARCHAR (255) NOT NULL,
    [RecentZoneName]    NVARCHAR (255) NULL,
    [ZoneChangeType]    INT            NOT NULL,
    [ZoneBED]           DATETIME       NOT NULL,
    [ZoneEED]           DATETIME       NULL
);




GO
CREATE CLUSTERED INDEX [IX_SaleZone_Preview_ProcessInstanceID]
    ON [TOneWhs_CP].[SaleZone_Preview]([ProcessInstanceID] ASC);

