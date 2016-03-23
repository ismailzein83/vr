﻿CREATE TABLE [bp].[BPDefinitionState] (
    [DefinitionID] INT            NOT NULL,
    [ObjectKey]    VARCHAR (255)  NOT NULL,
    [ObjectValue]  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_BPDefinitionState] PRIMARY KEY CLUSTERED ([DefinitionID] ASC, [ObjectKey] ASC)
);



