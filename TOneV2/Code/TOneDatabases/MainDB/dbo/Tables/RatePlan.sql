CREATE TABLE [dbo].[RatePlan] (
    [RatePlanID]         INT           IDENTITY (1, 1) NOT NULL,
    [CustomerID]         VARCHAR (5)   NOT NULL,
    [Description]        TEXT          NULL,
    [CurrencyID]         VARCHAR (3)   NULL,
    [BeginEffectiveDate] SMALLDATETIME NULL,
    [UserID]             INT           NULL,
    [timestamp]          ROWVERSION    NULL,
    CONSTRAINT [PK_RatePlan] PRIMARY KEY CLUSTERED ([RatePlanID] ASC),
    CONSTRAINT [FK_RatePlan_CarrierAccount] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_RatePlan_Customer]
    ON [dbo].[RatePlan]([CustomerID] ASC);

