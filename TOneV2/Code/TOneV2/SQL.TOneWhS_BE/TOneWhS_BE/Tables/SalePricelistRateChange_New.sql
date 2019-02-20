﻿CREATE TABLE [TOneWhS_BE].[SalePricelistRateChange_New] (
    [PricelistId]         INT             NOT NULL,
    [Rate]                DECIMAL (20, 8) NOT NULL,
    [RateTypeId]          INT             NULL,
    [RecentCurrencyId]    INT             NULL,
    [RecentRate]          DECIMAL (20, 8) NULL,
    [RecentRateConverted] DECIMAL (20, 8) NULL,
    [CountryID]           INT             NOT NULL,
    [ZoneName]            NVARCHAR (150)  NOT NULL,
    [ZoneID]              BIGINT          NULL,
    [Change]              INT             NULL,
    [ProcessInstanceID]   INT             NULL,
    [BED]                 DATETIME        NOT NULL,
    [EED]                 DATETIME        NULL,
    [RoutingProductID]    INT             NULL,
    [CurrencyID]          INT             NULL
);
















GO
CREATE CLUSTERED INDEX [IX_SalePricelistRateChange_New_ProcessInstanceID]
    ON [TOneWhS_BE].[SalePricelistRateChange_New]([ProcessInstanceID] ASC);



