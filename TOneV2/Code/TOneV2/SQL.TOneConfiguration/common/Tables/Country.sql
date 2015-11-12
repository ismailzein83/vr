CREATE TABLE [common].[Country] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (255) NOT NULL,
    [timestamp] ROWVERSION     NULL,
    CONSTRAINT [PK_Common.Country] PRIMARY KEY CLUSTERED ([ID] ASC)
);

