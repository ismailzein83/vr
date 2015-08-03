CREATE TABLE [dbo].[CodeChangeLog] (
    [ID]              INT            IDENTITY (1, 1) NOT NULL,
    [Description]     TEXT           NULL,
    [Updated]         SMALLDATETIME  NULL,
    [SourceFileBytes] IMAGE          NULL,
    [SourceFileName]  NVARCHAR (100) NULL,
    [UserID]          INT            NULL,
    [timestamp]       ROWVERSION     NULL,
    [StateBackUpID]   INT            NULL,
    [IsRestored]      BIT            NULL,
    CONSTRAINT [PK_ChangeLog] PRIMARY KEY CLUSTERED ([ID] ASC)
);

