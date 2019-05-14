CREATE TABLE [genericdata].[VRNumberPrefix] (
    [Id]               INT              IDENTITY (1, 1) NOT NULL,
    [Number]           VARCHAR (20)     NULL,
    [Type]             UNIQUEIDENTIFIER NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_VRNumberPrefix] PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Number] ASC)
);

