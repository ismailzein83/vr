CREATE TABLE [common].[Country] (
    [ID]        INT            NOT NULL,
    [Name]      NVARCHAR (255) NOT NULL,
    [SourceID]  VARCHAR (255)  NULL,
    [timestamp] ROWVERSION     NULL,
    CONSTRAINT [PK_Common.Country] PRIMARY KEY CLUSTERED ([ID] ASC)
);



