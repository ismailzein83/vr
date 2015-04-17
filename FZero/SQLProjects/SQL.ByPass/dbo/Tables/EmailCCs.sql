CREATE TABLE [dbo].[EmailCCs] (
    [Id]               INT          IDENTITY (1, 1) NOT NULL,
    [Email]            VARCHAR (50) NOT NULL,
    [MobileOperatorID] INT          NULL,
    [ClientID]         INT          NOT NULL,
    CONSTRAINT [PK_EmailCCs] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmailCCs_Clients] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Clients] ([ID]),
    CONSTRAINT [FK_EmailCCs_MobileOperators] FOREIGN KEY ([MobileOperatorID]) REFERENCES [dbo].[MobileOperators] ([ID])
);

