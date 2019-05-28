CREATE TABLE [CRMFixedOper].[Customer_Corporate] (
    [ID]                 BIGINT         NOT NULL,
    [ParentCorporateID]  BIGINT         NULL,
    [Name]               NVARCHAR (900) NULL,
    [RegistrationNumber] NVARCHAR (255) NULL,
    [PostalCode]         NVARCHAR (255) NULL,
    [WebSite]            NVARCHAR (255) NULL,
    [NbOfEmployees]      INT            NULL,
    [Attachments]        NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_CorporateCustomer] PRIMARY KEY CLUSTERED ([ID] ASC)
);

