CREATE TABLE [dbo].[ToneFeedbacks] (
    [ID]   INT           IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ToneFeedbacks] PRIMARY KEY CLUSTERED ([ID] ASC)
);

