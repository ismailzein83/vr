CREATE TABLE [TOneWhS_BE].[SalePricelistRPChange_New] (
    [ZoneName]               NVARCHAR (MAX) NULL,
    [ZoneID]                 BIGINT         NULL,
    [RoutingProductId]       INT            NULL,
    [RecentRoutingProductId] INT            NULL,
    [BED]                    DATETIME       NULL,
    [EED]                    DATETIME       NULL,
    [PriceListId]            BIGINT         NULL,
    [CountryId]              INT            NULL,
    [ProcessInstanceID]      INT            NULL,
    [CustomerId]             INT            NOT NULL,
    [LastModifiedTime]       DATETIME       CONSTRAINT [DF_SalePricelistRPChange_New_LastModifiedTime] DEFAULT (getdate()) NULL,
    [CreatedTime]            DATETIME       CONSTRAINT [DF_SalePricelistRPChange_New_CreatedTime] DEFAULT (getdate()) NULL
);












GO
CREATE NONCLUSTERED INDEX [IX_SalePricelistRPChange_New_Zone]
    ON [TOneWhS_BE].[SalePricelistRPChange_New]([ZoneID] ASC);


GO
CREATE CLUSTERED INDEX [IX_SalePricelistRPChange_New_ProcessInstanceID]
    ON [TOneWhS_BE].[SalePricelistRPChange_New]([ProcessInstanceID] ASC);

