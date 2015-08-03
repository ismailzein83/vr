CREATE TABLE [dbo].[tmpRatesImportedPricelist_1cqzifa4402fiv55woqwagzl] (
    [RateID]             BIGINT         NULL,
    [PriceListID]        INT            NULL,
    [ZoneID]             INT            NULL,
    [ZoneName]           VARCHAR (100)  NOT NULL,
    [Rate]               DECIMAL (9, 5) NULL,
    [OffPeakRate]        DECIMAL (9, 5) NULL,
    [WeekendRate]        DECIMAL (9, 5) NULL,
    [Change]             SMALLINT       NULL,
    [ServicesFlag]       SMALLINT       NULL,
    [BeginEffectiveDate] SMALLDATETIME  NULL,
    [EndEffectiveDate]   SMALLDATETIME  NULL,
    [UserID]             INT            NULL
);

