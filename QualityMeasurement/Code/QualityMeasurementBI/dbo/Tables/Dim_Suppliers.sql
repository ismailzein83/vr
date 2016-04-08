CREATE TABLE [dbo].[Dim_Suppliers] (
    [Pk_SupplierId] INT            NOT NULL,
    [Name]          NVARCHAR (255) NULL,
    [Prefix]        NVARCHAR (255) NULL,
    CONSTRAINT [PK_Dim_Suppliers] PRIMARY KEY CLUSTERED ([Pk_SupplierId] ASC)
);

