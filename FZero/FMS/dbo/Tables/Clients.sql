CREATE TABLE [dbo].[Clients] (
    [ID]                   INT           IDENTITY (1, 1) NOT NULL,
    [Name]                 VARCHAR (50)  NOT NULL,
    [SendDailyReport]      BIT           NULL,
    [SendWeeklyReport]     BIT           NULL,
    [SendMonthlyReport]    BIT           NULL,
    [ClientReport]         BIT           NULL,
    [ClientEmail]          NVARCHAR (50) NULL,
    [GMT]                  INT           NULL,
    [CountryCode]          VARCHAR (50)  NULL,
    [PrefixLength]         INT           NULL,
    [FraudPrefix]          VARCHAR (50)  NULL,
    [Length]               INT           NULL,
    [ClientReportSecurity] BIT           NULL,
    [SecurityEmail]        NVARCHAR (50) NULL,
    CONSTRAINT [PK_ClientNames] PRIMARY KEY CLUSTERED ([ID] ASC)
);





