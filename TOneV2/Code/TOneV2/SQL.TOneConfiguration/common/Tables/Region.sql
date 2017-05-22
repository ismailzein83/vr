CREATE TABLE [common].[Region] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [CountryID]   INT            NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_Region_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_Region_CountryName] UNIQUE NONCLUSTERED ([CountryID] ASC, [Name] ASC)
);

