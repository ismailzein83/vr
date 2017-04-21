CREATE TABLE [genericdata].[DataRecordFieldChoice] (
    [ID]         UNIQUEIDENTIFIER NOT NULL,
    [Name]       NVARCHAR (255)   NOT NULL,
    [Settings]   NVARCHAR (MAX)   NULL,
    [CreateTime] DATETIME         CONSTRAINT [DF_DataRecordFieldChoice_CreateTime] DEFAULT (getdate()) NULL,
    [timestamp]  ROWVERSION       NULL,
    CONSTRAINT [PK_DataRecordFieldChoice_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);



