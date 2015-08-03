CREATE TABLE [dbo].[Log] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Date]      DATETIME       NOT NULL,
    [Thread]    VARCHAR (32)   NOT NULL,
    [Context]   VARCHAR (512)  NULL,
    [Level]     VARCHAR (512)  NOT NULL,
    [Logger]    VARCHAR (512)  NOT NULL,
    [Message]   VARCHAR (4000) NOT NULL,
    [Exception] VARCHAR (2000) NULL,
    [timestamp] ROWVERSION     NOT NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_LogDate]
    ON [dbo].[Log]([Date] ASC);

