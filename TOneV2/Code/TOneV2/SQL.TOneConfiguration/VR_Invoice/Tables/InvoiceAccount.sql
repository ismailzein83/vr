CREATE TABLE [VR_Invoice].[InvoiceAccount] (
    [ID]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [InvoiceTypeID] UNIQUEIDENTIFIER NOT NULL,
    [PartnerID]     VARCHAR (50)     NOT NULL,
    [BED]           DATETIME         NULL,
    [EED]           DATETIME         NULL,
    [Status]        INT              NOT NULL,
    [IsDeleted]     BIT              NULL,
    [CreatedTime]   DATETIME         CONSTRAINT [DF_InvoicePartner_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]     ROWVERSION       NULL,
    CONSTRAINT [PK_InvoicePartner] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_InvoicePartner_TypeAndPartnerID] UNIQUE NONCLUSTERED ([InvoiceTypeID] ASC, [PartnerID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_InvoicePartner_StatusAndEffective]
    ON [VR_Invoice].[InvoiceAccount]([IsDeleted] ASC, [Status] ASC, [BED] ASC, [EED] ASC);

