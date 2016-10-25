CREATE TABLE [TOneWhS_BE_Bkup].[SupplierZone] (
    [ID]            BIGINT         NOT NULL,
    [CountryID]     INT            NOT NULL,
    [Name]          NVARCHAR (255) NOT NULL,
    [SupplierID]    INT            NOT NULL,
    [BED]           DATETIME       NOT NULL,
    [EED]           DATETIME       NULL,
    [SourceID]      VARCHAR (50)   NULL,
    [StateBackupID] BIGINT         NOT NULL
);








GO
CREATE CLUSTERED INDEX [IX_SupplierZone_StateBackupID]
    ON [TOneWhS_BE_Bkup].[SupplierZone]([StateBackupID] ASC);

