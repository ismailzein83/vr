CREATE TABLE [NIM].[Site] (
    [Id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [Area]             BIGINT         NULL,
    [Technology]       BIGINT         NULL,
    [Type]             BIGINT         NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    [Street]           NVARCHAR (255) NULL,
    [Building]         NVARCHAR (255) NULL,
    [BuildingSize]     INT            NULL,
    [BlockNumber]      NVARCHAR (255) NULL,
    [Region]           INT            NULL,
    [City]             INT            NULL,
    [Town]             INT            NULL,
    CONSTRAINT [PK__Site__3214EC0703317E3D] PRIMARY KEY CLUSTERED ([Id] ASC)
);

