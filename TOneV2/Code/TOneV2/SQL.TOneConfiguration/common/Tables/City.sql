CREATE TABLE [common].[City] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [CountryID]   INT            NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_City_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Common.City] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_City_CountryName] UNIQUE NONCLUSTERED ([CountryID] ASC, [Name] ASC)
);



