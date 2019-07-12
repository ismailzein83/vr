CREATE TABLE [RetailBilling].[CDR] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [ResourceName]     NVARCHAR (255)   NULL,
    [Contract]         BIGINT           NULL,
    [Direction]        INT              NULL,
    [OtherPartyNumber] VARCHAR (50)     NULL,
    [AttemptDateTime]  DATETIME         NULL,
    [Duration]         DECIMAL (20, 4)  NULL,
    [Currency]         INT              NULL,
    [Amount]           DECIMAL (22, 6)  NULL,
    [Type]             UNIQUEIDENTIFIER NULL
);




GO
CREATE CLUSTERED INDEX [IX_RetailBilling_CDR_AttemptDateTime]
    ON [RetailBilling].[CDR]([AttemptDateTime] ASC);

