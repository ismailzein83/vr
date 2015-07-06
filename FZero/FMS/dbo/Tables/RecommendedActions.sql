CREATE TABLE [dbo].[RecommendedActions] (
    [ID]   INT           IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_RecommendedActions] PRIMARY KEY CLUSTERED ([ID] ASC)
);

