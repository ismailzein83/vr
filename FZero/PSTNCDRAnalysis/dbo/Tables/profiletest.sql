CREATE TABLE [dbo].[profiletest] (
    [SubscriberNumber]   VARCHAR (30)    NULL,
    [Date_Day]           DATETIME        NULL,
    [Day_Hour]           INT             NULL,
    [Count_out]          INT             NULL,
    [Count_in]           INT             NOT NULL,
    [Count_out_Fail]     INT             NULL,
    [Count_in_Fail]      INT             NOT NULL,
    [Call_out_Dur_AVG]   NUMERIC (38, 6) NULL,
    [Call_in_Dur_Avg]    INT             NOT NULL,
    [total_out_volume]   NUMERIC (38, 4) NULL,
    [total_in_volume]    NUMERIC (38, 4) NOT NULL,
    [diff_output_numb_]  INT             NULL,
    [diff_input_numbers] INT             NOT NULL,
    [diff_dest_codes]    INT             NULL,
    [diff_sources_codes] INT             NOT NULL,
    [diff_out_type]      INT             NULL,
    [diff_in_type]       INT             NOT NULL
);

