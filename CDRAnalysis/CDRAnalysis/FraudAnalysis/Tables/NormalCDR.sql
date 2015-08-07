CREATE TABLE [FraudAnalysis].[NormalCDR] (
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
    [Service_VAS_Name]   VARCHAR (50)    NULL
);


GO
CREATE CLUSTERED INDEX [IX_NormalCDR_MSISDN]
    ON [FraudAnalysis].[NormalCDR]([MSISDN] ASC, [ConnectDateTime] ASC);

