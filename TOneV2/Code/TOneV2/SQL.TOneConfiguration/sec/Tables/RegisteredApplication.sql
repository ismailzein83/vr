CREATE TABLE [sec].[RegisteredApplication] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [Name]      VARCHAR (50)     NOT NULL,
    [URL]       VARCHAR (50)     NOT NULL,
    [timestamp] ROWVERSION       NOT NULL,
    CONSTRAINT [PK_sec.Application] PRIMARY KEY CLUSTERED ([Id] ASC)
);

