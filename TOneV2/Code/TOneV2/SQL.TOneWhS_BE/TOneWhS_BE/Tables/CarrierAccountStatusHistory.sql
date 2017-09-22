CREATE TABLE [TOneWhS_BE].[CarrierAccountStatusHistory] (
    [ID]                INT      IDENTITY (1, 1) NOT NULL,
    [CarrierAccountID]  INT      NOT NULL,
    [StatusID]          INT      NOT NULL,
    [PreviousStatusID]  INT      NULL,
    [StatusChangedDate] DATETIME NOT NULL,
    CONSTRAINT [PK_CarrierAccountStatusHistory] PRIMARY KEY CLUSTERED ([ID] ASC)
);

