CREATE TABLE [sec].[Module] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (255) NOT NULL,
    [Title]        NVARCHAR (255) NULL,
    [Url]          NVARCHAR (255) NULL,
    [ParentId]     INT            NULL,
    [Icon]         NVARCHAR (50)  NULL,
    [Rank]         INT            NULL,
    [AllowDynamic] INT            NULL,
    CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED ([Id] ASC)
);





