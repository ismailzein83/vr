CREATE TABLE [TOneWhS_BE].[SupplierRecurringChargesType] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK_SupplierRecurringChargesType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

