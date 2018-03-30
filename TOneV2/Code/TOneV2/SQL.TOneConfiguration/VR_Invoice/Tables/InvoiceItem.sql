CREATE TABLE [VR_Invoice].[InvoiceItem] (
    [ID]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [InvoiceID]   BIGINT         NOT NULL,
    [ItemSetName] NVARCHAR (255) NULL,
    [Name]        NVARCHAR (900) NOT NULL,
    [Details]     NVARCHAR (MAX) NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_InvoiceItem_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_InvoiceItem] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);










GO



GO



GO
CREATE CLUSTERED INDEX [IX_InvoiceItem_InvoiceItemSetName]
    ON [VR_Invoice].[InvoiceItem]([InvoiceID] ASC, [ItemSetName] ASC);



