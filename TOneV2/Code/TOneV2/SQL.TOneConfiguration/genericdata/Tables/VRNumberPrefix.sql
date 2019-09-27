CREATE TABLE [genericdata].[VRNumberPrefix] (
    [Id]               INT              IDENTITY (1, 1) NOT NULL,
    [Number]           VARCHAR (20)     NULL,
    [Type]             UNIQUEIDENTIFIER NULL,
    [IsExact]          BIT              NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_VRNumberPrefix] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ__VRNumber__78A1A19D7DE38492] UNIQUE NONCLUSTERED ([Number] ASC)
);



