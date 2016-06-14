CREATE TABLE [Retail].[Zone] (
    [ID]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (255) NOT NULL,
    [CountryId]   INT           NOT NULL,
    [CreatedTime] DATETIME      CONSTRAINT [DF_Zone_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [timestamp]   ROWVERSION    NULL,
    CONSTRAINT [PK_Zone] PRIMARY KEY CLUSTERED ([ID] ASC)
);

