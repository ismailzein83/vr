CREATE TABLE [sec].[BusinessEntityModule] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (255) NOT NULL,
    [Title]             NVARCHAR (255) NOT NULL,
    [ParentId]          INT            NULL,
    [BreakInheritance]  BIT            NOT NULL,
    [PermissionOptions] NVARCHAR (255) NULL,
    [timestamp]         ROWVERSION     NULL,
    CONSTRAINT [PK_BusinessEntityModule] PRIMARY KEY CLUSTERED ([Id] ASC)
);







