CREATE TABLE [TOneWhS_BE].[StateBackup] (
    [ID]          BIGINT          NOT NULL,
    [Description] NVARCHAR (1000) NOT NULL,
    [Info]        NVARCHAR (MAX)  NOT NULL,
    [BackupDate]  DATETIME        NOT NULL,
    [RestoreDate] DATETIME        NULL,
    CONSTRAINT [PK_StateBackup] PRIMARY KEY CLUSTERED ([ID] ASC)
);

