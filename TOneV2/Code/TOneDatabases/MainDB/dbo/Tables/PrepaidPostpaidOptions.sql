CREATE TABLE [dbo].[PrepaidPostpaidOptions] (
    [PrepaidPostpaidID]          INT             IDENTITY (1, 1) NOT NULL,
    [CustomerID]                 VARCHAR (10)    NULL,
    [CustomerProfileID]          INT             NULL,
    [SupplierID]                 VARCHAR (10)    NULL,
    [SupplierProfileID]          INT             NULL,
    [IsCustomer]                 CHAR (1)        CONSTRAINT [DF_PrepaidPostpaidOptions_IsCustomer] DEFAULT ('Y') NULL,
    [Amount]                     NUMERIC (13, 5) NULL,
    [Actions]                    SMALLINT        NULL,
    [Percentage]                 NUMERIC (18, 2) NULL,
    [IsPrepaid]                  CHAR (1)        NULL,
    [Email]                      VARCHAR (255)   NULL,
    [MinimumActionEmailInterval] VARCHAR (50)    NULL,
    [BlockAsCustomer]            CHAR (1)        NULL,
    [timestamp]                  ROWVERSION      NOT NULL,
    CONSTRAINT [PK_PrepaidPostpaidOptions] PRIMARY KEY CLUSTERED ([PrepaidPostpaidID] ASC)
);

