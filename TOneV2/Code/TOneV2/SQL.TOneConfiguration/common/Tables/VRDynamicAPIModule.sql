﻿CREATE TABLE [common].[VRDynamicAPIModule] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             VARCHAR (255)    NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_VRDynamicAPIModule_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_VRDynamicAPIModule] PRIMARY KEY CLUSTERED ([ID] ASC)
);







