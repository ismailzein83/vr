﻿CREATE TABLE [dbo].[Users] (
    [ID]               INT             IDENTITY (1, 1) NOT NULL,
    [FullName]         NVARCHAR (100)  NOT NULL,
    [EmailAddress]     NVARCHAR (100)  NOT NULL,
    [UserName]         NVARCHAR (100)  NOT NULL,
    [Password]         NVARCHAR (500)  NOT NULL,
    [LastLoginTime]    DATETIME2 (0)   NULL,
    [IsActive]         BIT             NOT NULL,
    [IsSuperUser]      BIT             CONSTRAINT [DF_Users_IsSuperUser] DEFAULT ((0)) NOT NULL,
    [VerificationCode] NCHAR (6)       NULL,
    [AppTypeID]        INT             NULL,
    [Address]          NVARCHAR (500)  NULL,
    [MobileNumber]     NVARCHAR (50)   NULL,
    [MaxDailyCases]    INT             CONSTRAINT [DF_Users_MaxDailyCases] DEFAULT ((0)) NOT NULL,
    [Mobile]           NVARCHAR (100)  NULL,
    [Prefix]           NVARCHAR (1000) NULL,
    [ContactName]      NVARCHAR (100)  NULL,
    [Website]          NVARCHAR (255)  NULL,
    [Signature]        VARBINARY (MAX) NULL,
    [ClientID]         INT             NULL,
    [GMT]              INT             CONSTRAINT [DF_Users_GMT] DEFAULT ((3)) NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Users_Clients] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Clients] ([ID])
);

