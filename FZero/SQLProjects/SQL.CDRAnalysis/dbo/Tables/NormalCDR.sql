CREATE TABLE [dbo].[NormalCDR] (
    [Id]                 INT             IDENTITY (1, 1) NOT NULL,
    [MSISDN]             VARCHAR (30)    NULL,
    [IMSI]               VARCHAR (20)    NULL,
    [ConnectDateTime]    DATETIME        NULL,
    [Destination]        VARCHAR (40)    NULL,
    [DurationInSeconds]  NUMERIC (13, 4) NULL,
    [DisconnectDateTime] DATETIME        NULL,
    [Call_Class]         VARCHAR (50)    NULL,
    [IsOnNet]            TINYINT         NULL,
    [Call_Type]          INT             NULL,
    [Sub_Type]           VARCHAR (20)    NULL,
    [IMEI]               VARCHAR (20)    NULL,
    [BTS_Id]             INT             NULL,
    [Cell_Id]            VARCHAR (20)    NULL,
    [SwitchRecordId]     INT             NULL,
    [Up_Volume]          DECIMAL (18, 2) NULL,
    [Down_Volume]        DECIMAL (18, 2) NULL,
    [Cell_Latitude]      DECIMAL (18, 8) NULL,
    [Cell_Longitude]     DECIMAL (18, 8) NULL,
    [In_Trunk]           VARCHAR (20)    NULL,
    [Out_Trunk]          VARCHAR (20)    NULL,
    [Service_Type]       INT             NULL,
    [Service_VAS_Name]   VARCHAR (50)    NULL,
    CONSTRAINT [PK_MobileCDR] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 80, STATISTICS_NORECOMPUTE = ON)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'prepaid, postpaid ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NormalCDR', @level2type = N'COLUMN', @level2name = N'Sub_Type';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1: call , 2 :sms, 3: .. flags', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NormalCDR', @level2type = N'COLUMN', @level2name = N'Call_Type';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1 or Zero dependes on Call_Class', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NormalCDR', @level2type = N'COLUMN', @level2name = N'IsOnNet';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Zain, Korek ..', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NormalCDR', @level2type = N'COLUMN', @level2name = N'Call_Class';

