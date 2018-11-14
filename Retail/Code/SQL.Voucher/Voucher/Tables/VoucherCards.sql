CREATE TABLE [Voucher].[VoucherCards] (
    [ID]                  BIGINT          IDENTITY (1, 1) NOT NULL,
    [VoucherTypeID]       BIGINT          NULL,
    [Amount]              DECIMAL (22, 6) NULL,
    [SerialNumber]        NVARCHAR (1000) NULL,
    [ActivationDate]      DATETIME        NULL,
    [CreatedTime]         DATETIME        CONSTRAINT [DF_VoucherCards_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]           INT             NULL,
    [LastModifiedTime]    DATETIME        NULL,
    [LastModifiedBy]      INT             NULL,
    [UsedDate]            DATETIME        NULL,
    [ExpiryDate]          DATETIME        NULL,
    [PinCode]             VARCHAR (255)   NULL,
    [ActivationCode]      VARCHAR (255)   NULL,
    [GenerationVoucherId] BIGINT          NULL,
    [CurrencyId]          INT             NULL,
    [LockedBy]            NVARCHAR (255)  NULL,
    [LockedDate]          DATETIME        NULL,
    [UsedBy]              NVARCHAR (255)  NULL,
    CONSTRAINT [PK_VoucherCards] PRIMARY KEY CLUSTERED ([ID] ASC)
);

