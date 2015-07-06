CREATE TABLE [sec].[Module] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (255) NOT NULL,
    [Url]      NVARCHAR (255) NULL,
    [ParentId] INT            NULL,
    [Icon]     NVARCHAR (50)  NULL,
    CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED ([Id] ASC)
);

