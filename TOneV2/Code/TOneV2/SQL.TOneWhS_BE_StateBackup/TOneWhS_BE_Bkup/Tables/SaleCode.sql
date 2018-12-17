﻿CREATE TABLE [TOneWhS_BE_Bkup].[SaleCode] (
    [ID]                BIGINT       NOT NULL,
    [Code]              VARCHAR (20) NOT NULL,
    [ZoneID]            BIGINT       NOT NULL,
    [CodeGroupID]       INT          NULL,
    [BED]               DATETIME     NOT NULL,
    [EED]               DATETIME     NULL,
    [SourceID]          VARCHAR (50) NULL,
    [StateBackupID]     BIGINT       NOT NULL,
    [ProcessInstanceID] BIGINT       NULL,
    [LastModifiedTime]  DATETIME     NULL
);












GO
CREATE CLUSTERED INDEX [IX_SaleCode_StateBackupID]
    ON [TOneWhS_BE_Bkup].[SaleCode]([StateBackupID] ASC);

