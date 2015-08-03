CREATE TABLE [dbo].[DS_Deletion_log] (
    [Auto_ID]            INT           IDENTITY (1, 1) NOT NULL,
    [TableName]          NVARCHAR (50) NULL,
    [TableIdentityValue] INT           NOT NULL,
    [IsDeleted]          CHAR (1)      NOT NULL,
    [TimeStamp]          ROWVERSION    NOT NULL
);


GO
CREATE CLUSTERED INDEX [IX_DS_Deletion_log]
    ON [dbo].[DS_Deletion_log]([TableName] DESC, [TableIdentityValue] DESC);

