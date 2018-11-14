CREATE TABLE [Voucher].[VoucherType] (
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)  NULL,
    [Description]      NVARCHAR (1000) NULL,
    [Amount]           DECIMAL (22, 6) NULL,
    [CurrencyId]       INT             NULL,
    [Active]           BIT             NULL,
    [CreatedTime]      DATETIME        CONSTRAINT [DF_VoucherType_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME        NULL,
    [CreatedBy]        INT             NULL,
    [LastModifiedBy]   INT             NULL,
    [timestamp]        ROWVERSION      NULL,
    CONSTRAINT [PK_VoucherType_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);

