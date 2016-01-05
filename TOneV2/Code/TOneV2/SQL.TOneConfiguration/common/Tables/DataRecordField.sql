CREATE TABLE [common].[DataRecordField] (
    [ID]        INT           IDENTITY (1, 1) NOT NULL,
    [TypeID]    INT           NULL,
    [Details]   VARCHAR (MAX) NULL,
    [timestamp] ROWVERSION    NULL,
    CONSTRAINT [PK_DataRecordField] PRIMARY KEY CLUSTERED ([ID] ASC)
);

