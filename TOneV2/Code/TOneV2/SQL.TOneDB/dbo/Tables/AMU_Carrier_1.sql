CREATE TABLE [dbo].[AMU_Carrier] (
    [ID]                 INT             IDENTITY (1, 1) NOT NULL,
    [CarrierAccountID]   VARCHAR (5)     NULL,
    [MctID]              INT             NULL,
    [AmuID]              INT             NULL,
    [AssignDate]         DATE            NULL,
    [AMUCarrierType]     TINYINT         NULL,
    [CustomerCommission] NUMERIC (19, 7) NULL,
    [SupplierCommission] NUMERIC (19, 7) NULL,
    CONSTRAINT [PK_AMU_Carrier] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MCT_AMU_Carrier] FOREIGN KEY ([MctID]) REFERENCES [dbo].[MCT] ([ID]) ON DELETE SET NULL
);

