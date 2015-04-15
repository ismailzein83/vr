CREATE TABLE [dbo].[EmailReceiverTypes] (
    [Id]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_EmailReceiverTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

