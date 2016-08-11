CREATE TABLE [common].[VRObjectTypeDefinition] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Settings]    NVARCHAR (MAX)   NULL,
    [timestamp]   ROWVERSION       NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_VRObjectTypeDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_VRObjectTypeDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);

