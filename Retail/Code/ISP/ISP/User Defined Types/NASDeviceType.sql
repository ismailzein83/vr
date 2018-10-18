CREATE TYPE [ISP].[NASDeviceType] AS TABLE (
    [ID]               BIGINT         NULL,
    [Name]             NVARCHAR (255) NULL,
    [IPAddress]        VARCHAR (50)   NULL,
    [Port]             INT            NULL,
    [Username]         NVARCHAR (50)  NULL,
    [Password]         NVARCHAR (50)  NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedTime] DATETIME       NULL);

