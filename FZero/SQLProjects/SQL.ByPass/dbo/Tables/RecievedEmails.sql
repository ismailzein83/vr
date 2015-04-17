CREATE TABLE [dbo].[RecievedEmails] (
    [ID]        INT           IDENTITY (1, 1) NOT NULL,
    [DateSent]  DATETIME2 (0) NOT NULL,
    [MessageID] VARCHAR (500) NOT NULL,
    [ReadDate]  DATETIME2 (0) NOT NULL,
    [SourceID]  INT           NOT NULL,
    CONSTRAINT [PK_RecievedEmails] PRIMARY KEY CLUSTERED ([ID] ASC)
);

