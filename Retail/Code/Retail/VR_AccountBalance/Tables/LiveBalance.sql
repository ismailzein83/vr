﻿CREATE TABLE [VR_AccountBalance].[LiveBalance] (
    [AccountID]             BIGINT          NOT NULL,
    [CurrencyID]            INT             NOT NULL,
    [InitialBalance]        DECIMAL (20, 6) NOT NULL,
    [UsageBalance]          DECIMAL (20, 6) NOT NULL,
    [CurrentBalance]        DECIMAL (20, 6) NOT NULL,
    [CurrentAlertThreshold] DECIMAL (20, 6) NULL,
    [NextAlertThreshold]    DECIMAL (20, 6) NULL,
    [AlertRuleID]           BIGINT          NULL,
    [timestamp]             ROWVERSION      NULL,
    CONSTRAINT [PK_LiveBalance] PRIMARY KEY CLUSTERED ([AccountID] ASC)
);



