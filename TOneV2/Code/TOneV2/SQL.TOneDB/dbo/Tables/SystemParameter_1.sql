CREATE TABLE [dbo].[SystemParameter] (
    [Name]          VARCHAR (50)    NOT NULL,
    [Type]          TINYINT         NOT NULL,
    [BooleanValue]  CHAR (1)        NULL,
    [NumericValue]  DECIMAL (20, 8) NULL,
    [TimeSpanValue] VARCHAR (50)    NULL,
    [DateTimeValue] DATETIME        NULL,
    [TextValue]     NVARCHAR (MAX)  NULL,
    [LongTextValue] NTEXT           NULL,
    [Description]   NTEXT           NULL,
    [UserID]        INT             NULL,
    [timestamp]     ROWVERSION      NOT NULL,
    [DS_ID_auto]    INT             IDENTITY (1, 1) NOT NULL
);

