CREATE TABLE [genericdata].[BEParentChildRelation] (
    [ID]                   BIGINT           IDENTITY (1, 1) NOT NULL,
    [RelationDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [ParentBEID]           VARCHAR (50)     NOT NULL,
    [ChildBEID]            VARCHAR (50)     NOT NULL,
    [BED]                  DATETIME         NOT NULL,
    [EED]                  DATETIME         NULL,
    [timestamp]            ROWVERSION       NULL,
    [CreatedTime]          DATETIME         CONSTRAINT [DF_BEParentChildRelation_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_BEParentChildRelation] PRIMARY KEY CLUSTERED ([ID] ASC)
);

