CREATE TABLE [dbo].[CellProfile] (
    [Id]                    INT          IDENTITY (1, 1) NOT NULL,
    [Cell_Id]               VARCHAR (20) NULL,
    [Date_Day]              DATETIME     NULL,
    [Day_Hour]              INT          NULL,
    [Distinct_MSISDN_Calls] INT          NULL,
    [Distinct_IMEI]         INT          NULL,
    [Distinct_MSISDN_Msg]   INT          NULL,
    CONSTRAINT [PK_CellProfile] PRIMARY KEY CLUSTERED ([Id] ASC)
);

