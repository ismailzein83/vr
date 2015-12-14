CREATE TABLE [common].[City] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (255) NOT NULL,
    [CountryID] INT            NOT NULL,
    [timestamp] ROWVERSION     NULL,
    CONSTRAINT [PK_Common.City] PRIMARY KEY CLUSTERED ([ID] ASC)
);

