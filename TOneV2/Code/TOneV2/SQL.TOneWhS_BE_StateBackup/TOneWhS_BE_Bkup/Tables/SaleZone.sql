﻿CREATE TABLE [TOneWhS_BE_Bkup].[SaleZone] (
    [ID]                  BIGINT         NOT NULL,
    [SellingNumberPlanID] INT            NOT NULL,
    [CountryID]           INT            NOT NULL,
    [Name]                NVARCHAR (255) NOT NULL,
    [BED]                 DATETIME       NOT NULL,
    [EED]                 DATETIME       NULL,
    [SourceID]            VARCHAR (50)   NULL,
    [StateBackupID]       BIGINT         NOT NULL,
    [ProcessInstanceID]   BIGINT         NULL,
    [LastModifiedTime]    DATETIME       NULL
);












GO
CREATE CLUSTERED INDEX [IX_SaleZone_StateBackupID]
    ON [TOneWhS_BE_Bkup].[SaleZone]([StateBackupID] ASC);

