CREATE TABLE [NetworkRentalManager].[AccountService] (
    [ID]                 BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountName]        NVARCHAR (255)   NULL,
    [Service]            NVARCHAR (255)   NULL,
    [NumberOfConnection] BIGINT           NULL,
    [RatePerConnection]  DECIMAL (26, 10) NULL,
    [Revenue]            DECIMAL (26, 10) NULL,
    [CreatedTime]        DATETIME         CONSTRAINT [DF_AccountService_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_AccountService] PRIMARY KEY CLUSTERED ([ID] ASC)
);

