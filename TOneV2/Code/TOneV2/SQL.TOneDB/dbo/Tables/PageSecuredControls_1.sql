CREATE TABLE [dbo].[PageSecuredControls] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [ControlId]         VARCHAR (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [ControlType]       VARCHAR (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [ParentControlId]   VARCHAR (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [ParentControlType] VARCHAR (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [CommandName]       VARCHAR (50)  COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [ColumnIndex]       INT           NULL,
    [PermissionId]      VARCHAR (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [PropertyAction]    VARCHAR (50)  COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [PagePath]          VARCHAR (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [Details]           VARCHAR (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    CONSTRAINT [PK_PageSecuredControls] PRIMARY KEY CLUSTERED ([Id] ASC)
);

