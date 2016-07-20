CREATE TABLE [Retail_BE].[ActionDefinition] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Settings]    NVARCHAR (MAX)   NOT NULL,
    [EntityType]  INT              NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_ActionDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_ActionDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);



