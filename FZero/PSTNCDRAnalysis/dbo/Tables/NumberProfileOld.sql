CREATE TABLE [dbo].[NumberProfileOld] (
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
    [Diff_In_Type]       INT             NOT NULL,
    CONSTRAINT [PK_NumberProfile] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_number]
    ON [dbo].[NumberProfileOld]([SubscriberNumber] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_Day_Hour]
    ON [dbo].[NumberProfileOld]([Day_Hour] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_day_date]
    ON [dbo].[NumberProfileOld]([Date_Day] ASC) WITH (FILLFACTOR = 90);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Trunk Type the call passed out', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NumberProfileOld', @level2type = N'COLUMN', @level2name = N'Diff_Out_Type';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Zones of Incoming Calls', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NumberProfileOld', @level2type = N'COLUMN', @level2name = N'Diff_Sources_Codes';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Distinct Called Zones', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NumberProfileOld', @level2type = N'COLUMN', @level2name = N'Diff_Dest_Codes';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Distinct Called Numbers', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NumberProfileOld', @level2type = N'COLUMN', @level2name = N'Diff_Output_Numb_';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Minutes of Total Outgoing', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NumberProfileOld', @level2type = N'COLUMN', @level2name = N'Total_Out_Volume';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'number of minutes in each call as an average', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NumberProfileOld', @level2type = N'COLUMN', @level2name = N'Call_Out_Dur_Avg';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'same as count_in but with zero duration', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NumberProfileOld', @level2type = N'COLUMN', @level2name = N'Count_In_Fail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'same as count_out but with zero duration', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NumberProfileOld', @level2type = N'COLUMN', @level2name = N'Count_Out_Fail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Number of Incoming Calls', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NumberProfileOld', @level2type = N'COLUMN', @level2name = N'Count_In';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Number of Originated Calls', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NumberProfileOld', @level2type = N'COLUMN', @level2name = N'Count_Out';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'0-23, 25 means all day, 30 means all month', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NumberProfileOld', @level2type = N'COLUMN', @level2name = N'Day_Hour';

