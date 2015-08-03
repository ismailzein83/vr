CREATE TABLE [dbo].[RouteOption] (
    [RouteID]              INT         NOT NULL,
    [SupplierID]           VARCHAR (5) NOT NULL,
    [SupplierZoneID]       INT         NULL,
    [SupplierActiveRate]   REAL        NULL,
    [SupplierNormalRate]   REAL        NULL,
    [SupplierOffPeakRate]  REAL        NULL,
    [SupplierWeekendRate]  REAL        NULL,
    [SupplierServicesFlag] SMALLINT    NULL,
    [Priority]             TINYINT     NOT NULL,
    [NumberOfTries]        TINYINT     NULL,
    [State]                TINYINT     DEFAULT ((0)) NOT NULL,
    [Updated]              DATETIME    NULL,
    [Percentage]           TINYINT     NULL
);


GO
CREATE NONCLUSTERED INDEX [IDX_RouteOption_Updated]
    ON [dbo].[RouteOption]([Updated] DESC);


GO
CREATE NONCLUSTERED INDEX [IDX_RouteOption_SupplierZoneID]
    ON [dbo].[RouteOption]([SupplierZoneID] ASC);


GO
CREATE CLUSTERED INDEX [IDX_RouteOption_RouteID]
    ON [dbo].[RouteOption]([RouteID] ASC);

