CREATE TABLE [common].[Country] (
    [ID]               INT            NOT NULL,
    [Name]             NVARCHAR (255) NOT NULL,
    [SourceID]         VARCHAR (255)  NULL,
    [timestamp]        ROWVERSION     NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_Country_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    CONSTRAINT [PK_Common.Country] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_Country_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);







