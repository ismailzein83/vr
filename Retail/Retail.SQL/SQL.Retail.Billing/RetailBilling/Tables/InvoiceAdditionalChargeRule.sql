CREATE TABLE [RetailBilling].[InvoiceAdditionalChargeRule] (
    [Name]             NVARCHAR (255)  NULL,
    [Condition]        NVARCHAR (MAX)  NULL,
    [Settings]         INT             NULL,
    [Amount]           DECIMAL (22, 6) NULL,
    [Percentage]       INT             NULL,
    [CreatedTime]      DATETIME        NULL,
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [CreatedBy]        INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [LastModifiedBy]   INT             NULL
);

