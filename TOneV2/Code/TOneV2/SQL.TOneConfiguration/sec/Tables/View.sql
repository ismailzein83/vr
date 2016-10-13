CREATE TABLE [sec].[View] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Title]       NVARCHAR (255)   NULL,
    [Url]         NVARCHAR (255)   NULL,
    [Module]      UNIQUEIDENTIFIER NULL,
    [ActionNames] NVARCHAR (1000)  NULL,
    [Audience]    NVARCHAR (255)   NULL,
    [Content]     NVARCHAR (MAX)   NULL,
    [Settings]    NVARCHAR (MAX)   NULL,
    [Type]        UNIQUEIDENTIFIER NOT NULL,
    [OldType]     INT              NULL,
    [Rank]        INT              NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_View] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_View_Module] FOREIGN KEY ([Module]) REFERENCES [sec].[Module] ([ID])
);























