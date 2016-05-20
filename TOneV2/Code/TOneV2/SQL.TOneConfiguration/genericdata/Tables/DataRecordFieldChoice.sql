CREATE TABLE [genericdata].[DataRecordFieldChoice] (
    [ID]         INT            IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (255) NOT NULL,
    [Settings]   NVARCHAR (MAX) NULL,
    [CreateTime] DATETIME       CONSTRAINT [DF_DataRecordFieldChoice_CreateTime] DEFAULT (getdate()) NULL,
    [timestamp]  ROWVERSION     NULL,
    CONSTRAINT [PK_DataRecordFieldChoice] PRIMARY KEY CLUSTERED ([ID] ASC)
);

