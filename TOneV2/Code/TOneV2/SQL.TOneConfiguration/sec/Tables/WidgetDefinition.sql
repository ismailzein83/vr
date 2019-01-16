CREATE TABLE [sec].[WidgetDefinition] (
    [ID]               INT          IDENTITY (1, 1) NOT NULL,
    [Name]             NCHAR (50)   NULL,
    [DirectiveName]    NCHAR (50)   NULL,
    [Setting]          NCHAR (1000) NULL,
    [timestamp]        ROWVERSION   NULL,
    [LastModifiedTime] DATETIME     CONSTRAINT [DF_WidgetDefinition_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_WidgetDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);





