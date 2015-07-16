CREATE TABLE [sec].[Permission] (
    [HolderType]      INT            NOT NULL,
    [HolderId]        VARCHAR (50)   NOT NULL,
    [EntityType]      INT            NOT NULL,
    [EntityId]        VARCHAR (50)   NOT NULL,
    [PermissionFlags] VARCHAR (1000) NOT NULL,
    CONSTRAINT [PK_Permission_1] PRIMARY KEY CLUSTERED ([HolderType] ASC, [HolderId] ASC, [EntityType] ASC, [EntityId] ASC)
);

