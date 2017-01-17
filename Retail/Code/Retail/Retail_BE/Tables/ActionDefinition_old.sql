CREATE TABLE [Retail_BE].[ActionDefinition_old] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Settings]    NVARCHAR (MAX)   NOT NULL,
    [EntityType]  INT              NULL,
    [CreatedTime] DATETIME         NULL,
    [timestamp]   ROWVERSION       NULL
);

