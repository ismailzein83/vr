CREATE TABLE [TOneWhS_BE].[SalePricelistRPChange] (
    [ZoneName]               NVARCHAR (MAX) NULL,
    [ZoneID]                 BIGINT         NULL,
    [RoutingProductId]       INT            NULL,
    [RecentRoutingProductId] INT            NULL,
    [BED]                    DATETIME       NULL,
    [EED]                    DATETIME       NULL,
    [PriceListId]            BIGINT         NULL,
    [CountryId]              INT            NULL,
    [CustomerId]             INT            NULL
);








GO
CREATE CLUSTERED INDEX [IX_SalePricelistRPChange_PriceListID]
    ON [TOneWhS_BE].[SalePricelistRPChange]([PriceListId] ASC);

