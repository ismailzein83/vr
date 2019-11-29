﻿CREATE TABLE [NIM].[Area] (
    [Id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (100) NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    [RegionID]         INT            NULL,
    [CityID]           INT            NULL,
    CONSTRAINT [PK__Area__3214EC077F60ED59] PRIMARY KEY CLUSTERED ([Id] ASC)
);





