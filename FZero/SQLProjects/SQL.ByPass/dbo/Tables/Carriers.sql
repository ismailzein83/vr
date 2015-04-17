CREATE TABLE [dbo].[Carriers] (
    [ID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (50) NULL,
    CONSTRAINT [PK_Carriers] PRIMARY KEY CLUSTERED ([ID] ASC)
);

