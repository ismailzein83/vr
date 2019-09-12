﻿CREATE TABLE [VRNotification].[VRAlertRuleType] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_VRAlertRuleType_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_VRAlertRuleType_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_VRAlertRuleType] PRIMARY KEY CLUSTERED ([ID] ASC)
);





