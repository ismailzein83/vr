CREATE TABLE [dbo].[EmailTokens] (
    [ID]    INT           IDENTITY (1, 1) NOT NULL,
    [Token] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_EmailTokens] PRIMARY KEY CLUSTERED ([ID] ASC)
);

