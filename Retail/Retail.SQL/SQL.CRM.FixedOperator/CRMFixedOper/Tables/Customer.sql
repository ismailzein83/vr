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
    [AddressNotes]            NVARCHAR (MAX)   NULL,
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
    [StatusReason]            INT              NULL,
    [PhoneNumbers]            NVARCHAR (MAX)   NULL,
    [Emails]                  NVARCHAR (MAX)   NULL,
    [ContactName]             NVARCHAR (255)   NULL,
    [ContactNumber]           NVARCHAR (255)   NULL,
    [DifferentBillingAddress] BIT              NULL,
    [BillingRegionID]         INT              NULL,
    [BillingCityID]           INT              NULL,
    [BillingTownID]           INT              NULL,
    [BillingStreet]           NVARCHAR (255)   NULL,
    [BillingBuilding]         NVARCHAR (255)   NULL,
    [BillingBuildingSize]     INT              NULL,
    [BillingBlockNumber]      NVARCHAR (255)   NULL,
    [BillingFloorNumber]      INT              NULL,
    [BillingFloorSide]        INT              NULL,
    [BillingAddressNotes]     NVARCHAR (MAX)   NULL,
    [SocialMediaAddresses]    NVARCHAR (MAX)   NULL,
    [DunningStatus]           UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID] ASC)
);











