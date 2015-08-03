CREATE TABLE [dbo].[tmpRates_3abiw5zk1434ze55wmavwo55] (
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

