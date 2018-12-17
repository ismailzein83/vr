CREATE TABLE [bp].[BPDefinitionArgumentState] (
    [BPDefinitionID]   UNIQUEIDENTIFIER NOT NULL,
    [InputArgument]    NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_BPDefintionArgumentState_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_BPDefinitionArgumentState_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_BPDefintionArgumentState] PRIMARY KEY CLUSTERED ([BPDefinitionID] ASC)
);

