﻿CREATE TABLE [dbo].[MobileOperators] (
    [ID]            INT          IDENTITY (1, 1) NOT NULL,
    [UserID]        INT          NOT NULL,
    [Code]          VARCHAR (10) NULL,
    [AutoReport]    BIT          CONSTRAINT [DF_MobileOperators_AutoReport] DEFAULT ((0)) NOT NULL,
    [RepeatedCases] BIT          CONSTRAINT [DF_MobileOperators_RepeatedCases] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Customers_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Customers_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID])
);

