CREATE TABLE [PATool].[ActivitySubCategory] (
    [ID]                 UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (255)   NULL,
    [ActivityCategoryID] UNIQUEIDENTIFIER NULL,
    [CreatedTime]        DATETIME         NULL,
    [CreatedBy]          INT              NULL,
    [LastModifiedTime]   DATETIME         NULL,
    [LastModifiedBy]     INT              NULL,
    [timestamp]          ROWVERSION       NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

