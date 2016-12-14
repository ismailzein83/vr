CREATE TABLE [QM_BE].[Supplier] (
    [ID]               INT            NOT NULL,
    [Name]             NVARCHAR (255) NOT NULL,
    [Settings]         NVARCHAR (MAX) NULL,
    [SourceSupplierID] VARCHAR (255)  NULL,
    [timestamp]        ROWVERSION     NULL,
    [IsDeleted]        BIT            CONSTRAINT [DF_Supplier_IsDeleted] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED ([ID] ASC)
);







