CREATE TABLE [bp].[BPDefinitionState] (
    [DefinitionID]    UNIQUEIDENTIFIER NOT NULL,
    [ObjectKey]       VARCHAR (255)    NOT NULL,
    [OldDefinitionID] INT              NULL,
    [ObjectValue]     NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_BPDefinitionState] PRIMARY KEY CLUSTERED ([DefinitionID] ASC, [ObjectKey] ASC)
);





