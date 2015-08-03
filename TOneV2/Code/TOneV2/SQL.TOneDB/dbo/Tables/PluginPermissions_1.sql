CREATE TABLE [dbo].[PluginPermissions] (
    [ID]              VARCHAR (255)  NOT NULL,
    [Name]            NVARCHAR (255) NOT NULL,
    [Description]     NTEXT          NULL,
    [timestamp]       ROWVERSION     NULL,
    [PermissionLevel] TINYINT        NULL
);

