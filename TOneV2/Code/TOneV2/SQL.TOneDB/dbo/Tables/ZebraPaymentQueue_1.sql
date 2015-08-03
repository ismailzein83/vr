CREATE TABLE [dbo].[ZebraPaymentQueue] (
    [PaymentID]       INT             IDENTITY (1, 1) NOT NULL,
    [ZebraCustomerID] VARCHAR (5)     NULL,
    [Amount]          NUMERIC (13, 5) NULL,
    [CurrencyID]      VARCHAR (3)     NULL,
    [CreationDate]    DATETIME        NULL,
    [Tag]             VARCHAR (255)   NULL,
    [timestamp]       ROWVERSION      NOT NULL,
    [LastUpdate]      DATETIME        NULL,
    [ReferenceNumber] VARCHAR (50)    NULL,
    [Note]            VARCHAR (250)   NULL,
    CONSTRAINT [PK_ZebraPayment] PRIMARY KEY CLUSTERED ([PaymentID] ASC)
);

