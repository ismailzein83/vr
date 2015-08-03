CREATE TABLE [dbo].[ZebraSupplier] (
    [CarrierAccountID] VARCHAR (5)   NOT NULL,
    [SupplierName]     NVARCHAR (50) NULL,
    [Prefix]           VARCHAR (50)  NULL,
    [ActivationStatus] TINYINT       NULL,
    [TimeStamp]        ROWVERSION    NULL,
    [UserID]           INT           NULL,
    [LastUpdated]      DATETIME      NULL,
    [CreationDate]     DATETIME      NULL
);

