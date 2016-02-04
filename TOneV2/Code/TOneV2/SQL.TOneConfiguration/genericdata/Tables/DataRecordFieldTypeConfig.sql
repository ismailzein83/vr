CREATE TABLE [genericdata].[DataRecordFieldTypeConfig] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (255)  NOT NULL,
    [Details]     NVARCHAR (MAX) NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_DataRecordFieldType_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_DataRecordFieldType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_DataRecordFieldType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

