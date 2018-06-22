CREATE TYPE [sec].[RegisteredApplicationType] AS TABLE (
    [ID]               UNIQUEIDENTIFIER NULL,
    [Name]             VARCHAR (255)    NULL,
    [URL]              NVARCHAR (255)   NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL);

