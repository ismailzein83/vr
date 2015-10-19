CREATE TABLE [TOneWhS_BE].[CodeGroup] (
    [ID]        INT          IDENTITY (1, 1) NOT NULL,
    [CountryID] INT          NOT NULL,
    [Code]      VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_CodeGroup_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CodeGroup_Country] FOREIGN KEY ([CountryID]) REFERENCES [TOneWhS_BE].[Country] ([ID])
);

