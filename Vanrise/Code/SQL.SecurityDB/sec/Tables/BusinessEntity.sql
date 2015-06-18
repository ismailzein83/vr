CREATE TABLE [sec].[BusinessEntity] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (255) NOT NULL,
    [ModuleId]          INT            NOT NULL,
    [BreakInheritance]  BIT            NOT NULL,
    [PermissionOptions] NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_BusinessEntity] PRIMARY KEY CLUSTERED ([Id] ASC)
);

