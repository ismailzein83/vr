CREATE TABLE [dbo].[OnlinePaymentTransaction] (
    [TransactionID]   INT            IDENTITY (1, 1) NOT NULL,
    [CarrierID]       NVARCHAR (5)   NULL,
    [UserRef]         NVARCHAR (50)  NULL,
    [CardNumber]      NVARCHAR (20)  NULL,
    [ExpireDate]      NVARCHAR (4)   NULL,
    [Status]          NVARCHAR (3)   NULL,
    [Message]         NVARCHAR (200) NULL,
    [TransactionDate] DATETIME       NULL,
    [Amount]          FLOAT (53)     NULL,
    CONSTRAINT [PK_OnlinePaymentTransaction_1] PRIMARY KEY CLUSTERED ([TransactionID] ASC)
);

