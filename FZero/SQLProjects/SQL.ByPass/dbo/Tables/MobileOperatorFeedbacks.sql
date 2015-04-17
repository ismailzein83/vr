CREATE TABLE [dbo].[MobileOperatorFeedbacks] (
    [ID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_MobileOperatorFeedbacks] PRIMARY KEY CLUSTERED ([ID] ASC)
);

