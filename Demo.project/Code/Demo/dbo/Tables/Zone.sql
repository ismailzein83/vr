CREATE TABLE [dbo].[Zone] (
    [ID]        BIGINT         NOT NULL,
    [CountryID] INT            NOT NULL,
    [Name]      NVARCHAR (255) NOT NULL,
    [BED]       DATETIME       NOT NULL,
    [EED]       DATETIME       NULL,
    [timestamp] ROWVERSION     NULL,
    CONSTRAINT [PK_SaleZone] PRIMARY KEY CLUSTERED ([ID] ASC)
);

