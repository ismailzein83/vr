CREATE TABLE [TOneWhS_BE].[Country] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (255) NOT NULL,
    [timestamp] ROWVERSION     NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_Country_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);



