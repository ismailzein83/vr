﻿CREATE TYPE [ICX_BE].[GatewayType] AS TABLE (
    [ID]               INT            NULL,
    [Name]             NVARCHAR (255) NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL);

