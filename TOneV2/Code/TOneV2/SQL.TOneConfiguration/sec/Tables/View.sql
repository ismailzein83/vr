CREATE TABLE [sec].[View] (
    [Id]                  INT             IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (255)  NOT NULL,
    [Title]               NVARCHAR (255)  NULL,
    [Url]                 NVARCHAR (255)  NOT NULL,
    [Module]              INT             NOT NULL,
    [RequiredPermissions] NVARCHAR (1000) NULL,
    [Audience]            NVARCHAR (255)  NULL,
    [Content]             NVARCHAR (MAX)  NULL,
    [Type]                INT             NOT NULL,
    [Rank]                INT             NULL,
    CONSTRAINT [PK_Views] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Module_View] FOREIGN KEY ([Module]) REFERENCES [sec].[Module] ([Id])
);



