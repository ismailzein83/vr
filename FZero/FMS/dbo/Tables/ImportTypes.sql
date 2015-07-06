CREATE TABLE [dbo].[ImportTypes] (
    [ID]   INT           IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ImportTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);

