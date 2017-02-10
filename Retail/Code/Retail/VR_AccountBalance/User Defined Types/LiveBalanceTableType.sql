CREATE TYPE [VR_AccountBalance].[LiveBalanceTableType] AS TABLE (
    [AccountID]   VARCHAR (50)    NOT NULL,
    [UpdateValue] DECIMAL (20, 6) NULL,
    PRIMARY KEY CLUSTERED ([AccountID] ASC));



