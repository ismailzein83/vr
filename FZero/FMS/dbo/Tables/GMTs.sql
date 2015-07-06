CREATE TABLE [dbo].[GMTs] (
    [ID]    INT          IDENTITY (1, 1) NOT NULL,
    [Name]  VARCHAR (50) NOT NULL,
    [Value] INT          NOT NULL,
    CONSTRAINT [PK_GMTs] PRIMARY KEY CLUSTERED ([ID] ASC)
);

