﻿CREATE TABLE [Mediation_Generic].[MediationDefinition] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID] UNIQUEIDENTIFIER NULL,
    [Name]         VARCHAR (255)    NOT NULL,
    [Details]      NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]  DATETIME         CONSTRAINT [DF_MediationSettingDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]    ROWVERSION       NULL,
    CONSTRAINT [PK_MediationDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);







