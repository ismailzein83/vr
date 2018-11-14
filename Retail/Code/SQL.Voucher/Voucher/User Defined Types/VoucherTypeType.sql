CREATE TYPE [Voucher].[VoucherTypeType] AS TABLE (
    [ID]               BIGINT          NULL,
    [Name]             NVARCHAR (255)  NULL,
    [Description]      NVARCHAR (1000) NULL,
    [Amount]           DECIMAL (22, 6) NULL,
    [CurrencyId]       INT             NULL,
    [Active]           BIT             NULL,
    [CreatedTime]      DATETIME        NULL,
    [LastModifiedTime] DATETIME        NULL,
    [CreatedBy]        INT             NULL,
    [LastModifiedBy]   INT             NULL);

