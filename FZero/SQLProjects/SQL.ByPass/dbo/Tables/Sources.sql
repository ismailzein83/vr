CREATE TABLE [dbo].[Sources] (
    [ID]             INT           IDENTITY (1, 1) NOT NULL,
    [Name]           VARCHAR (100) NOT NULL,
    [GMT]            INT           NOT NULL,
    [Email]          VARCHAR (50)  NULL,
    [SourceTypeID]   INT           NULL,
    [LastUpdateDate] DATETIME2 (0) NULL,
    [LastUpdatedBy]  INT           NULL,
    [SourceKindID]   INT           NULL,
    CONSTRAINT [PK_Sources] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Sources_SourceKinds] FOREIGN KEY ([SourceKindID]) REFERENCES [dbo].[SourceKinds] ([ID]),
    CONSTRAINT [FK_Sources_SourceTypes] FOREIGN KEY ([SourceTypeID]) REFERENCES [dbo].[SourceTypes] ([ID])
);

