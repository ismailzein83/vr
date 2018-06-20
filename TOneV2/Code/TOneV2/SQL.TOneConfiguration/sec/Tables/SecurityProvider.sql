CREATE TABLE [sec].[SecurityProvider] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_SecurityProvider_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_SecurityProvider_LastModifiedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_SecurityProvider] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_SecurityProvider_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

