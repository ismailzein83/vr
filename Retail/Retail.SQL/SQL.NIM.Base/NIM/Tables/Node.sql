CREATE TABLE [NIM].[Node] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Number]           NVARCHAR (450)   NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [ModelID]          INT              NULL,
    [Building]         NVARCHAR (255)   NULL,
    [BlockNumber]      NVARCHAR (255)   NULL,
    [RegionID]         INT              NULL,
    [CityID]           INT              NULL,
    [TownID]           INT              NULL,
    [SiteID]           INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [Notes]            NVARCHAR (MAX)   NULL,
    [StreetID]         BIGINT           NULL,
    [BuildingSizeID]   INT              NULL,
    CONSTRAINT [PK__Node__3214EC2722FF2F51] PRIMARY KEY CLUSTERED ([ID] ASC)
);

