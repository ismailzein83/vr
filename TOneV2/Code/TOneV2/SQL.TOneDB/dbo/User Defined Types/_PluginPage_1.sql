CREATE TYPE [dbo].[_PluginPage] AS TABLE (
    [PagePath]       NVARCHAR (255) NULL,
    [PluginName]     NVARCHAR (100) NULL,
    [PageName]       NVARCHAR (100) NULL,
    [PermissionRoot] NVARCHAR (255) NULL,
    [PermissionId]   NVARCHAR (255) NULL);

