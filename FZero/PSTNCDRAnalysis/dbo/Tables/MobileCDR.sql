CREATE TABLE [dbo].[MobileCDR] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [MSISDN]            VARCHAR (30)    NULL,
    [IMSI]              VARCHAR (20)    NULL,
    [Connect]           DATETIME        NULL,
    [CDPN]              VARCHAR (40)    NULL,
    [DurationInSeconds] NUMERIC (13, 4) NULL,
    [Disconnect]        DATETIME        NULL,
    [Call_Class]        VARCHAR (50)    NULL,
    [Call_Type]         VARCHAR (20)    NULL,
    [Sub_Type]          VARCHAR (20)    NULL,
    [IMEI]              VARCHAR (20)    NULL,
    [BTS_Id]            INT             NULL,
    [LAC]               VARCHAR (50)    NULL,
    [Cell_Id]           VARCHAR (20)    NULL,
    [Origin_Zone_Code]  VARCHAR (10)    NULL,
    [Termin_Zone_Code]  VARCHAR (10)    NULL,
    [Account_Age]       DATETIME        NULL,
    CONSTRAINT [PK_MobileCDR] PRIMARY KEY CLUSTERED ([Id] ASC)
);

