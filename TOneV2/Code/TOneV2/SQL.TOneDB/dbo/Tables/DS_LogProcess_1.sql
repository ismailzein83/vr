CREATE TABLE [dbo].[DS_LogProcess] (
    [RowID]             INT           IDENTITY (1, 1) NOT NULL,
    [SessionID]         BIGINT        NULL,
    [LogPhrase]         VARCHAR (500) NULL,
    [SecondsSinceStart] INT           NULL
);

