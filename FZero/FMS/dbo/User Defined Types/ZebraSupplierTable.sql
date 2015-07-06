CREATE TYPE [dbo].[ZebraSupplierTable] AS TABLE (
    [CarrierAccountID] VARCHAR (5)   NOT NULL,
    [SupplierName]     NVARCHAR (50) NULL,
    [Prefix]           VARCHAR (50)  NULL,
    [ActivationStatus] INT           NULL,
    [LastUpdated]      DATETIME      NULL);

