CREATE TYPE [RA].[SpecialNumbersType] AS TABLE (
    [ID]               BIGINT         NULL,
    [GroupName]        VARCHAR (MAX)  NULL,
    [Settings]         NVARCHAR (MAX) NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL);

