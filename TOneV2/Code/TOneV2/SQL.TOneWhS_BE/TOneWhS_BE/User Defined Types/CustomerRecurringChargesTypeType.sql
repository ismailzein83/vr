CREATE TYPE [TOneWhS_BE].[CustomerRecurringChargesTypeType] AS TABLE (
    [ID]               BIGINT         NULL,
    [Name]             NVARCHAR (255) NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL);

