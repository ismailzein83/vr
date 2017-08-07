CREATE TYPE [genericdata].[BEParentChildRelationType] AS TABLE (
    [RelationDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [ParentBEID]           VARCHAR (50)     NOT NULL,
    [ChildBEID]            VARCHAR (50)     NOT NULL,
    [BED]                  DATETIME         NOT NULL,
    [EED]                  DATETIME         NULL);

