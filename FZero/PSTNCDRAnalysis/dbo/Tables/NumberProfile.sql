CREATE TABLE [dbo].[NumberProfile] (
    [Id]                 INT             IDENTITY (1, 1) NOT NULL,
    [SubscriberNumber]   VARCHAR (30)    NULL,
    [Date_Day]           DATETIME        NULL,
    [Day_Hour]           INT             NULL,
    [Count_Out]          INT             NULL,
    [Count_In]           INT             NOT NULL,
    [Count_Out_Fail]     INT             NULL,
    [Count_In_Fail]      INT             NOT NULL,
    [Call_Out_Dur_Avg]   NUMERIC (38, 6) NULL,
    [Call_In_Dur_Avg]    INT             NOT NULL,
    [Total_Out_Volume]   NUMERIC (38, 4) NULL,
    [Total_In_Volume]    INT             NOT NULL,
    [Diff_Output_Numb_]  INT             NULL,
    [Diff_Input_Numbers] INT             NOT NULL,
    [Diff_Dest_Codes]    INT             NULL,
    [Diff_Sources_Codes] INT             NOT NULL,
    [Diff_Out_Type]      INT             NULL,
    [Diff_In_Type]       INT             NOT NULL
);

