CREATE TYPE [genericdata].[VRNumberPrefixType] AS TABLE (
    [Id]               INT              NULL,
    [Number]           VARCHAR (20)     NULL,
    [Type]             UNIQUEIDENTIFIER NULL,
    [IsExact]          BIT              NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL);



