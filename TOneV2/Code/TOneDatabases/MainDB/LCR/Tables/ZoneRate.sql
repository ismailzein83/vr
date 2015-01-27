CREATE TABLE [LCR].[ZoneRate] (
    [RateID]             BIGINT         NOT NULL,
    [ZoneID]             INT            NOT NULL,
    [SupplierID]         VARCHAR (5)    NOT NULL,
    [CustomerID]         VARCHAR (5)    NOT NULL,
    [NormalRate]         DECIMAL (9, 5) NULL,
    [ServicesFlag]       SMALLINT       NULL,
    [ProfileId]          INT            NULL,
    [BeginEffectiveDate] SMALLDATETIME  NOT NULL,
    [EndEffectiveDate]   SMALLDATETIME  NULL,
    [IsEffective]        AS             (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
    [timestamp]          ROWVERSION     NULL,
    [PricelistID]        INT            NULL,
    [ZoneName]           NVARCHAR (255) NULL,
    CONSTRAINT [PK_ZoneRate] PRIMARY KEY CLUSTERED ([RateID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_LCRZoneRate_ZoneID]
    ON [LCR].[ZoneRate]([ZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LCRZoneRate_SupplierID]
    ON [LCR].[ZoneRate]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LCRZoneRate_ServicesFlag]
    ON [LCR].[ZoneRate]([ServicesFlag] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LCRZoneRate_EffectiveRate]
    ON [LCR].[ZoneRate]([NormalRate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LCRZoneRate_EED]
    ON [LCR].[ZoneRate]([EndEffectiveDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LCRZoneRate_CustomerID]
    ON [LCR].[ZoneRate]([CustomerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LCRZoneRate_BED]
    ON [LCR].[ZoneRate]([BeginEffectiveDate] ASC);

