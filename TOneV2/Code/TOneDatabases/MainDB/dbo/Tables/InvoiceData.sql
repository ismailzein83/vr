CREATE TABLE [dbo].[InvoiceData] (
    [SourceFilebytes] IMAGE      NULL,
    [InvoiceID]       INT        NOT NULL,
    [timestamp]       ROWVERSION NULL,
    [DS_ID_auto]      INT        IDENTITY (1, 1) NOT NULL
);

