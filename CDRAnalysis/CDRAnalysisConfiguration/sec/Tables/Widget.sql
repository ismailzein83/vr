CREATE TABLE [sec].[Widget] (
    [Id]                 INT             IDENTITY (1, 1) NOT NULL,
    [WidgetDefinitionId] INT             NULL,
    [Name]               NVARCHAR (50)   NULL,
    [Title]              NVARCHAR (50)   NULL,
    [Setting]            NVARCHAR (1000) NULL,
    CONSTRAINT [PK_WidgetManagement] PRIMARY KEY CLUSTERED ([Id] ASC)
);

