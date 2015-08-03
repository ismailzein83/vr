CREATE TABLE [dbo].[mdrouteoption] (
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
    [State]                TINYINT     NOT NULL,
    [Updated]              DATETIME    NULL,
    [percentage]           TINYINT     NULL
);

