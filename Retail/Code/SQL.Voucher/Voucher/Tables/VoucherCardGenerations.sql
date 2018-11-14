CREATE TABLE [Voucher].[VoucherCardGenerations] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [VoucherTypeId]    BIGINT         NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_VoucherCardGenerations_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [NumberOfCards]    INT            NULL,
    [ExpiryDate]       DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    [InactiveCards]    INT            NULL,
    CONSTRAINT [PK_VoucherCardGenerations] PRIMARY KEY CLUSTERED ([ID] ASC)
);

