CREATE TABLE [TOneWhS_BE_Bkup].[CustomerCountry] (
    [ID]            BIGINT   NOT NULL,
    [CustomerID]    INT      NOT NULL,
    [CountryID]     INT      NOT NULL,
    [BED]           DATETIME NOT NULL,
    [EED]           DATETIME NULL,
    [StateBackupID] BIGINT   NOT NULL
);

