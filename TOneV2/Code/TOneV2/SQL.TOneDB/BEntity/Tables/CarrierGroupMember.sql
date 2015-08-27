CREATE TABLE [BEntity].[CarrierGroupMember] (
    [CarrierGroupID]   INT         NOT NULL,
    [CarrierAccountID] VARCHAR (5) NOT NULL,
    [timestamp]        ROWVERSION  NULL,
    CONSTRAINT [PK_CarrierGroupCarrierAccount] PRIMARY KEY CLUSTERED ([CarrierGroupID] ASC, [CarrierAccountID] ASC)
);

