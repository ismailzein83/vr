CREATE TABLE [sec].[RegisteredApplication] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             VARCHAR (50)     NOT NULL,
    [URL]              VARCHAR (50)     NOT NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_RegisteredApplication_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NOT NULL,
    CONSTRAINT [PK_RegisteredApplication] PRIMARY KEY CLUSTERED ([Id] ASC)
);



