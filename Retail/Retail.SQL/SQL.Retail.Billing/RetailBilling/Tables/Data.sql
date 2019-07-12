CREATE TABLE [RetailBilling].[Data] (
    [Id]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [ResourceName]    NVARCHAR (255)  NULL,
    [Contract]        BIGINT          NULL,
    [AttemptDateTime] DATETIME        NULL,
    [Volume]          DECIMAL (22, 2) NULL,
    [Currency]        INT             NULL,
    [Amount]          DECIMAL (22, 6) NULL
);


GO
CREATE CLUSTERED INDEX [IX_RetailBilling_Data_AttemptDateTime]
    ON [RetailBilling].[Data]([AttemptDateTime] ASC);

