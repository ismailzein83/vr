CREATE TABLE [dbo].[CodeSupply] (
    [Code]                 NVARCHAR (30) NOT NULL,
    [SupplierID]           VARCHAR (5)   NULL,
    [SupplierZoneID]       INT           NOT NULL,
    [SupplierNormalRate]   REAL          NULL,
    [SupplierOffPeakRate]  REAL          NULL,
    [SupplierWeekendRate]  REAL          NULL,
    [SupplierServicesFlag] SMALLINT      CONSTRAINT [DF_CodeSupply_SupplierServicesFlag] DEFAULT ((0)) NULL,
    [Updated]              DATETIME      CONSTRAINT [DF_CodeSupply_Updated] DEFAULT (getdate()) NOT NULL,
    [ProfileId]            INT           NULL,
    [Blocked]              TINYINT       CONSTRAINT [DF_CodeSupply_Blocked] DEFAULT ((0)) NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_CodeSupply_Zone]
    ON [dbo].[CodeSupply]([SupplierZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodeSupply_Supplier]
    ON [dbo].[CodeSupply]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodeSupply_Code]
    ON [dbo].[CodeSupply]([Code] ASC);

