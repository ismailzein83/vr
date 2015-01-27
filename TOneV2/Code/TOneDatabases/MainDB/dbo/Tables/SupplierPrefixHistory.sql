CREATE TABLE [dbo].[SupplierPrefixHistory] (
    [ID]           INT           IDENTITY (1, 1) NOT NULL,
    [SupplierID]   VARCHAR (5)   NULL,
    [Prefix]       NVARCHAR (50) NULL,
    [Creationdate] DATETIME      NULL,
    [UserID]       INT           NULL,
    CONSTRAINT [PK_SupplierPrefixHistory] PRIMARY KEY CLUSTERED ([ID] ASC)
);

