CREATE TABLE [dbo].[ZoneRate] (
    [ZoneID]       INT         NOT NULL,
    [SupplierID]   VARCHAR (5) NOT NULL,
    [CustomerID]   VARCHAR (5) NOT NULL,
    [NormalRate]   REAL        NULL,
    [OffPeakRate]  REAL        NULL,
    [WeekendRate]  REAL        NULL,
    [ServicesFlag] SMALLINT    NULL,
    [ProfileId]    INT         NULL,
    [Blocked]      TINYINT     CONSTRAINT [DF_ZoneRate_Blocked] DEFAULT ((0)) NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_ZoneRate_Zone]
    ON [dbo].[ZoneRate]([ZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZoneRate_Supplier]
    ON [dbo].[ZoneRate]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZoneRate_ServicesFlag]
    ON [dbo].[ZoneRate]([ServicesFlag] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZoneRate_Customer]
    ON [dbo].[ZoneRate]([CustomerID] ASC);

