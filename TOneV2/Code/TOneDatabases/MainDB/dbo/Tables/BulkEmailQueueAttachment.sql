CREATE TABLE [dbo].[BulkEmailQueueAttachment] (
    [ID]            INT             IDENTITY (1, 1) NOT NULL,
    [QueueID]       INT             NOT NULL,
    [FileName]      VARCHAR (100)   NULL,
    [FileExtension] VARCHAR (10)    NULL,
    [Attachment]    VARBINARY (MAX) NULL,
    [ContentType]   VARCHAR (250)   NULL,
    [timestamp]     ROWVERSION      NULL,
    CONSTRAINT [PK_BulkEmailQueueAttachment_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);

