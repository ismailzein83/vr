CREATE TABLE [dbo].[ApplicationUsers] (
    [ID]     INT IDENTITY (1, 1) NOT NULL,
    [UserID] INT NOT NULL,
    CONSTRAINT [PK_AdminUsers] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Administrators_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID])
);

