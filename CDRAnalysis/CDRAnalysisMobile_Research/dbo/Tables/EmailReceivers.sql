CREATE TABLE [dbo].[EmailReceivers] (
    [Id]                  INT          IDENTITY (1, 1) NOT NULL,
    [Email]               VARCHAR (50) NOT NULL,
    [EmailTemplateID]     INT          NULL,
    [EmailReceiverTypeID] INT          NULL,
    CONSTRAINT [PK_EmailCCs] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmailReceivers_EmailReceiverTypes] FOREIGN KEY ([EmailReceiverTypeID]) REFERENCES [dbo].[EmailReceiverTypes] ([Id]),
    CONSTRAINT [FK_EmailReceivers_EmailTemplates] FOREIGN KEY ([EmailTemplateID]) REFERENCES [dbo].[EmailTemplates] ([ID])
);

