CREATE TABLE [common].[City] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NOT NULL,
    [CountryID]        INT            NOT NULL,
    [Settings]         NVARCHAR (MAX) NULL,
    [SourceId]         VARCHAR (50)   NULL,
    [timestamp]        ROWVERSION     NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_City_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    CONSTRAINT [PK_Common.City] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_City_CountryName] UNIQUE NONCLUSTERED ([CountryID] ASC, [Name] ASC)
);









