CREATE TABLE [dbo].[Facts] (
    [MS_CDRId]              INT             NULL,
    [MS_IMEI]               VARCHAR (20)    NULL,
    [MS_MSISDN]             VARCHAR (30)    NULL,
    [MS_CaseId]             INT             NULL,
    [MS_Duration]           DECIMAL (13, 4) NULL,
    [FK_CallClass]          INT             NULL,
    [FK_CallType]           INT             NULL,
    [FK_CaseStatus]         INT             NULL,
    [FK_NetworkType]        INT             NULL,
    [FK_Period]             INT             NULL,
    [FK_Strategy]           INT             NULL,
    [FK_StrategyKind]       INT             NULL,
    [FK_SubscriberType]     VARCHAR (20)    NULL,
    [FK_SuspicionLevel]     INT             NULL,
    [FK_ConnectTime]        DATETIME        NULL,
    [FK_CaseGenerationTime] DATETIME        NULL,
    [FK_StrategyUser]       INT             NULL,
    [FK_CaseUser]           INT             NULL,
    [FK_BTS]                INT             NULL
);

