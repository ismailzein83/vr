CREATE TABLE [TOneWhS_BE_Bkup].[CustomerCountry] (
    [ID]                BIGINT   NOT NULL,
    [CustomerID]        INT      NOT NULL,
    [CountryID]         INT      NOT NULL,
    [BED]               DATETIME NOT NULL,
    [EED]               DATETIME NULL,
    [StateBackupID]     BIGINT   NOT NULL,
    [ProcessInstanceID] BIGINT   NULL
);






GO
CREATE CLUSTERED INDEX [IX_CustomerCountry_StateBackupID]
    ON [TOneWhS_BE_Bkup].[CustomerCountry]([StateBackupID] ASC);

