CREATE TABLE [sec].[BusinessEntityModule] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NOT NULL,
    [ParentId]         INT            NULL,
    [BreakInheritance] BIT            NOT NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK_BusinessEntityModule] PRIMARY KEY CLUSTERED ([Id] ASC)
);









