CREATE TABLE [VR_Invoice].[InvoiceGenerationDraft] (
    [ID]                          BIGINT           IDENTITY (1, 1) NOT NULL,
    [InvoiceGenerationIdentifier] UNIQUEIDENTIFIER NULL,
    [InvoiceTypeId]               UNIQUEIDENTIFIER NOT NULL,
    [PartnerID]                   VARCHAR (50)     NOT NULL,
    [PartnerName]                 VARCHAR (MAX)    NOT NULL,
    [FromDate]                    DATETIME         NOT NULL,
    [ToDate]                      DATETIME         NOT NULL,
    [CustomPayload]               NVARCHAR (MAX)   NULL,
    [CreatedTime]                 DATETIME         CONSTRAINT [DF_InvoiceGenerationDraft_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_InvoiceGenerationDraft] PRIMARY KEY CLUSTERED ([ID] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_InvoiceGenerationDraft_InvoiceGenerationIdentifier]
    ON [VR_Invoice].[InvoiceGenerationDraft]([InvoiceGenerationIdentifier] ASC);

