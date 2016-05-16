CREATE TABLE [TOneWhS_BE].[Switch] (
    [ID]        INT          IDENTITY (1, 1) NOT NULL,
    [Name]      VARCHAR (50) NOT NULL,
    [timestamp] ROWVERSION   NULL,
    [SourceID]  VARCHAR (50) NULL,
    CONSTRAINT [PK_Switch] PRIMARY KEY CLUSTERED ([ID] ASC)
);





