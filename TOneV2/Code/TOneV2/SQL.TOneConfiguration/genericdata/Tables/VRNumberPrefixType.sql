CREATE TABLE [genericdata].[VRNumberPrefixType] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             VARCHAR (255)    NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_VRNumberPrefixType] PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Name] ASC)
);

