CREATE TABLE [QM_BE].[Zone] (
    [ID]           BIGINT         NOT NULL,
    [CountryID]    INT            NOT NULL,
    [Name]         NVARCHAR (255) NOT NULL,
    [BED]          DATETIME       NOT NULL,
    [EED]          DATETIME       NULL,
    [SourceZoneID] VARCHAR (50)   NULL,
    [timestamp]    ROWVERSION     NULL,
    CONSTRAINT [PK_Zone] PRIMARY KEY CLUSTERED ([ID] ASC)
);



