CREATE TABLE [common].[DataRecordType] (
    [ID]        INT          IDENTITY (1, 1) NOT NULL,
    [Name]      VARCHAR (50) NULL,
    [ParentID]  INT          NULL,
    [timestamp] ROWVERSION   NULL,
    CONSTRAINT [PK_DataRecordType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

