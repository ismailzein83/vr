CREATE TABLE [CentralLog].[EventNode] (
    [ID]               INT              IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [NodeTypeID]       UNIQUEIDENTIFIER NULL,
    [timestamp]        ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

