CREATE TABLE [dbo].[ActionTypes] (
    [ID]          INT            NOT NULL,
    [Name]        NVARCHAR (300) NOT NULL,
    [Description] NVARCHAR (500) NOT NULL,
    CONSTRAINT [PK_Actions] PRIMARY KEY CLUSTERED ([ID] ASC)
);

