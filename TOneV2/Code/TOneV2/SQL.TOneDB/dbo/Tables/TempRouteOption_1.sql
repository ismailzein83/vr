CREATE TABLE [dbo].[TempRouteOption] (
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

