CREATE TABLE [genericdata].[DataRecordType] (
    [ID]          INT             IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (1000) NOT NULL,
    [ParentID]    INT             NULL,
    [Fields]      NVARCHAR (MAX)  NULL,
    [CreatedTime] DATETIME        CONSTRAINT [DF_DataRecordType_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION      NULL,
    CONSTRAINT [PK_DataRecordType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

