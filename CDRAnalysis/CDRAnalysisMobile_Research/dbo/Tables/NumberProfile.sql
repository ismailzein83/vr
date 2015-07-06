﻿CREATE TABLE [dbo].[NumberProfile] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [SubscriberNumber]          VARCHAR (30)   CONSTRAINT [DF__ts_Number__Subsc__6DB73E6A] DEFAULT (NULL) NULL,
    [FromDate]                  DATETIME       CONSTRAINT [DF__ts_Number__FromD__6EAB62A3] DEFAULT (NULL) NULL,
    [ToDate]                    DATETIME       CONSTRAINT [DF__ts_Number__ToDat__6F9F86DC] DEFAULT (NULL) NULL,
    [Count_Out_Calls]           DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Count__7093AB15] DEFAULT (NULL) NULL,
    [Diff_Output_Numb]          DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Diff___7187CF4E] DEFAULT (NULL) NULL,
    [Count_Out_Inter]           DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Count__727BF387] DEFAULT (NULL) NULL,
    [Count_In_Inter]            DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Count__737017C0] DEFAULT (NULL) NULL,
    [InCalls_Vs_OutCalls]       DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__InCal__74643BF9] DEFAULT (NULL) NULL,
    [DiffDest_Vs_OutCalls]      DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__DiffD__75586032] DEFAULT (NULL) NULL,
    [OutInter_Vs_OutCalls]      DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__OutIn__764C846B] DEFAULT (NULL) NULL,
    [Call_Out_Dur_Avg]          DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Call___7740A8A4] DEFAULT (NULL) NULL,
    [AvrDurIn_Vs_AvrDurOut]     DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__AvrDu__7834CCDD] DEFAULT (NULL) NULL,
    [OutOffNet_Vs_OutOnNet]     DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__OutOf__7928F116] DEFAULT (NULL) NULL,
    [Count_Out_Fail]            DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Count__7A1D154F] DEFAULT (NULL) NULL,
    [Count_In_Fail]             DECIMAL (9, 2) NULL,
    [Total_Out_Volume]          DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Total__7B113988] DEFAULT (NULL) NULL,
    [Total_In_Volume]           DECIMAL (9, 2) NULL,
    [Diff_Input_Numbers]        DECIMAL (9, 2) NULL,
    [Count_Out_SMS]             DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Count__7C055DC1] DEFAULT (NULL) NULL,
    [Total_IMEI]                DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Total__7CF981FA] DEFAULT (NULL) NULL,
    [Total_BTS]                 DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Total__7DEDA633] DEFAULT (NULL) NULL,
    [IsOnNet]                   DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__IsOnN__7EE1CA6C] DEFAULT (NULL) NULL,
    [Total_Data_Volume]         DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Total__7FD5EEA5] DEFAULT (NULL) NULL,
    [PeriodId]                  DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Perio__00CA12DE] DEFAULT (NULL) NULL,
    [Count_In_Calls]            DECIMAL (9, 2) NULL,
    [Call_In_Dur_Avg]           DECIMAL (9, 2) NULL,
    [Count_Out_OnNet]           DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Count__01BE3717] DEFAULT (NULL) NULL,
    [Count_In_OnNet]            DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Count__02B25B50] DEFAULT (NULL) NULL,
    [Count_Out_OffNet]          DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Count__03A67F89] DEFAULT (NULL) NULL,
    [Count_In_OffNet]           DECIMAL (9, 2) CONSTRAINT [DF__ts_Number__Count__049AA3C2] DEFAULT (NULL) NULL,
    [CountFailConsecutiveCalls] DECIMAL (9, 2) NULL,
    [CountConsecutiveCalls]     DECIMAL (9, 2) NULL,
    [CountInLowDurationCalls]   DECIMAL (9, 2) NULL,
    CONSTRAINT [PK__ts_Numbe__3214EC076BCEF5F8] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_NumberProfile]
    ON [dbo].[NumberProfile]([FromDate] ASC, [ToDate] ASC);

