CREATE TABLE [CRMFixedOper].[Customer] (
    [ID]                      BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                    NVARCHAR (255)   NULL,
    [CustomerTypeID]          UNIQUEIDENTIFIER NULL,
    [CustomerCategoryID]      UNIQUEIDENTIFIER NULL,
    [CustomerSubCategoryID]   UNIQUEIDENTIFIER NULL,
    [CurrencyID]              INT              NULL,
    [StatusID]                UNIQUEIDENTIFIER NULL,
    [BillingCustomerID]       BIGINT           NULL,
    [DefaultBillingAccountID] BIGINT           NULL,
    [NumberOfBillingAccounts] INT              NULL,
    [FileNotes]               NVARCHAR (255)   NULL,
    [RegionID]                INT              NULL,
    [CityID]                  INT              NULL,
    [TownID]                  INT              NULL,
    [Street]                  NVARCHAR (255)   NULL,
    [Building]                NVARCHAR (255)   NULL,
    [FloorNumber]             INT              NULL,
    [AddressNotes]            NVARCHAR (255)   NULL,
    [PhoneNumber]             NVARCHAR (255)   NULL,
    [MobileNumber]            NVARCHAR (255)   NULL,
    [Email]                   NVARCHAR (255)   NULL,
    [FaxNumber]               NVARCHAR (255)   NULL,
    [FirstName]               NVARCHAR (255)   NULL,
    [MiddleName]              NVARCHAR (255)   NULL,
    [LastName]                NVARCHAR (255)   NULL,
    [SalutationID]            UNIQUEIDENTIFIER NULL,
    [Attachments]             NVARCHAR (MAX)   NULL,
    [CreatedTime]             DATETIME         NULL,
    [CreatedBy]               INT              NULL,
    [LastModifiedTime]        DATETIME         NULL,
    [LastModifiedBy]          INT              NULL,
    [FullName]                NVARCHAR (255)   NULL,
    [FloorSide]               INT              NULL,
    [BuildingSize]            INT              NULL,
    [BlockNumber]             NVARCHAR (255)   NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID] ASC)
);







