CREATE TYPE [TOneWhS_BE].[SupplierRecurringChargesTypeType] AS TABLE (
    [ID]               BIGINT         NULL,
    [Name]             NVARCHAR (255) NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL);

