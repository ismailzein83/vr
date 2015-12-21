CREATE TYPE [TOneWhS_BE].[UpdatedAccountManagerCarriersType] AS TABLE (
    [UserId]           INT         NOT NULL,
    [CarrierAccountId] VARCHAR (5) NOT NULL,
    [RelationType]     INT         NOT NULL,
    [Status]           BIT         NOT NULL);

