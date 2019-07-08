﻿CREATE TABLE [RetailBilling].[Customer] (
    [ID]                    BIGINT           IDENTITY (1, 1) NOT NULL,
    [ExternalCustomerID]    NVARCHAR (255)   NULL,
    [Name]                  NVARCHAR (255)   NULL,
    [CustomerTypeID]        UNIQUEIDENTIFIER NULL,
    [CustomerCategoryID]    UNIQUEIDENTIFIER NULL,
    [CustomerSubCategoryID] UNIQUEIDENTIFIER NULL,
    [StatusID]              UNIQUEIDENTIFIER NULL,
    [FileNotes]             NVARCHAR (255)   NULL,
    [RegionID]              INT              NULL,
    [CityID]                INT              NULL,
    [TownID]                INT              NULL,
    [StreetID]              NVARCHAR (255)   NULL,
    [Building]              NVARCHAR (255)   NULL,
    [FloorNumber]           INT              NULL,
    [AddressNotes]          NVARCHAR (255)   NULL,
    [PhoneNumber]           NVARCHAR (255)   NULL,
    [MobileNumber]          NVARCHAR (255)   NULL,
    [Email]                 NVARCHAR (255)   NULL,
    [FaxNumber]             NVARCHAR (255)   NULL,
    [FirstName]             NVARCHAR (255)   NULL,
    [MiddleName]            NVARCHAR (255)   NULL,
    [LastName]              NVARCHAR (255)   NULL,
    [SalutationID]          UNIQUEIDENTIFIER NULL,
    [CreatedTime]           DATETIME         NULL,
    [CreatedBy]             INT              NULL,
    [LastModifiedTime]      DATETIME         NULL,
    [LastModifiedBy]        INT              NULL,
    [DunningStatus]         UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__Customer__3214EC277A672E12] PRIMARY KEY CLUSTERED ([ID] ASC)
);



