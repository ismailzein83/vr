CREATE TABLE [RetailBilling].[CDR] (
    [Id]               BIGINT           NULL,
    [Contract]         BIGINT           NULL,
    [Direction]        INT              NULL,
    [OtherPartyNumber] VARCHAR (50)     NULL,
    [AttemptDateTime]  DATETIME         NULL,
    [Duration]         DECIMAL (20, 4)  NULL,
    [Currency]         INT              NULL,
    [Amount]           DECIMAL (22, 6)  NULL,
    [Type]             UNIQUEIDENTIFIER NULL
);

