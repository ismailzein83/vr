CREATE TABLE [dbo].[IranStringCDR] (
    [ID]               BIGINT        IDENTITY (1, 1) NOT NULL,
    [CallRecordType]   NVARCHAR (50) NULL,
    [SubscriberMSISDN] NVARCHAR (50) NULL,
    [CallPartner]      NVARCHAR (50) NULL,
    [CallDate]         NVARCHAR (50) NULL,
    [CallTime]         NVARCHAR (50) NULL,
    [CallDuration]     NVARCHAR (50) NULL,
    [MSLocation]       NVARCHAR (50) NULL,
    [IMEI]             NVARCHAR (50) NULL,
    CONSTRAINT [PK_IranStringCDR] PRIMARY KEY CLUSTERED ([ID] ASC)
);

