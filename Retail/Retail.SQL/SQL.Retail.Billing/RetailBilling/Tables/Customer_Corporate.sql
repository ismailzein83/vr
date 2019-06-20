CREATE TABLE [RetailBilling].[Customer_Corporate] (
    [ID]                 BIGINT         NOT NULL,
    [ParentCorporate]    BIGINT         NULL,
    [RegistrationNumber] NVARCHAR (255) NULL,
    [WebSite]            NVARCHAR (255) NULL,
    [PostalCode]         NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

