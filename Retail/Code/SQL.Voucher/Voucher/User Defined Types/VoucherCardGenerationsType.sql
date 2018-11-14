CREATE TYPE [Voucher].[VoucherCardGenerationsType] AS TABLE (
    [ID]               BIGINT         NULL,
    [Name]             NVARCHAR (255) NULL,
    [VoucherTypeId]    BIGINT         NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [NumberOfCards]    INT            NULL,
    [ExpiryDate]       DATETIME       NULL,
    [InactiveCards]    INT            NULL);

