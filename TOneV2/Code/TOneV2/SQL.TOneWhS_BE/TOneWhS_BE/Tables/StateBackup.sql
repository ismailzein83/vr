CREATE TABLE [TOneWhS_BE].[StateBackup] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [ConfigID]         UNIQUEIDENTIFIER NULL,
    [Info]             NVARCHAR (MAX)   NOT NULL,
    [BackupDate]       DATETIME         NOT NULL,
    [RestoreDate]      DATETIME         NULL,
    [BackupByUserID]   INT              NOT NULL,
    [RestoredByUserID] INT              NULL,
    CONSTRAINT [PK_StateBackup] PRIMARY KEY CLUSTERED ([ID] ASC)
);







