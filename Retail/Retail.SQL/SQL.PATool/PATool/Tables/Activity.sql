CREATE TABLE [PATool].[Activity] (
    [ID]                    BIGINT           IDENTITY (1, 1) NOT NULL,
    [ClientID]              INT              NULL,
    [ProjectID]             INT              NULL,
    [ProductID]             INT              NULL,
    [ActivityCategoryID]    UNIQUEIDENTIFIER NULL,
    [ActivitySubCategoryID] UNIQUEIDENTIFIER NULL,
    [Description]           NVARCHAR (255)   NULL,
    [Date]                  DATE             NULL,
    [NumberOfHours]         INT              NULL,
    [CreatedTime]           DATETIME         NULL,
    [CreatedBy]             INT              NULL,
    [LastModifiedTime]      DATETIME         NULL,
    [LastModifiedBy]        INT              NULL,
    [Name]                  NVARCHAR (255)   NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

