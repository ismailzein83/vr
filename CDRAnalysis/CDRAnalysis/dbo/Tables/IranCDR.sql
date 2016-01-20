CREATE TABLE [dbo].[IranCDR] (
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [CallRecordType]   INT             NULL,
    [SubscriberMSISDN] NVARCHAR (50)   NULL,
    [CallPartner]      NVARCHAR (50)   NULL,
    [CallDate]         DATETIME        NULL,
    [CallTime]         TIME (7)        NULL,
    [CallDuration]     DECIMAL (18, 2) NULL,
    [MSLocation]       NVARCHAR (50)   NULL,
    [IMEI]             NVARCHAR (50)   NULL,
    CONSTRAINT [PK_IranCDR] PRIMARY KEY CLUSTERED ([ID] ASC)
);

