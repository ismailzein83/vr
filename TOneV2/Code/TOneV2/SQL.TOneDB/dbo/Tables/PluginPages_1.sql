CREATE TABLE [dbo].[PluginPages] (
    [PagePath]       NVARCHAR (255) NOT NULL,
    [PluginName]     NVARCHAR (100) NOT NULL,
    [PageName]       NVARCHAR (100) NOT NULL,
    [PermissionRoot] NVARCHAR (255) NOT NULL,
    [PermissionId]   NVARCHAR (255) NOT NULL
);

