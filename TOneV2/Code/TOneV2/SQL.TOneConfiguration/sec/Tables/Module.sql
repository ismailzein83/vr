CREATE TABLE [sec].[Module] (
    [ID]            UNIQUEIDENTIFIER NOT NULL,
    [Name]          NVARCHAR (255)   NOT NULL,
    [Url]           NVARCHAR (255)   NULL,
    [DefaultViewId] UNIQUEIDENTIFIER NULL,
    [ParentId]      UNIQUEIDENTIFIER NULL,
    [Icon]          NVARCHAR (50)    NULL,
    [Rank]          INT              NULL,
    [AllowDynamic]  BIT              NULL,
    [Settings]      NVARCHAR (MAX)   NULL,
    [timestamp]     ROWVERSION       NULL,
    CONSTRAINT [PK_Module_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Module_Module] FOREIGN KEY ([ID]) REFERENCES [sec].[Module] ([ID])
);









