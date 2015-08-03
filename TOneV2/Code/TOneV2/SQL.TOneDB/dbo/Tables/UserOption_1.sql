CREATE TABLE [dbo].[UserOption] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [User]      INT            NULL,
    [Option]    NVARCHAR (255) NOT NULL,
    [Value]     NVARCHAR (MAX) NULL,
    [timestamp] ROWVERSION     NULL,
    CONSTRAINT [PK_UserOption] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_UserOption_User] FOREIGN KEY ([User]) REFERENCES [dbo].[User] ([ID]) ON DELETE CASCADE
);

