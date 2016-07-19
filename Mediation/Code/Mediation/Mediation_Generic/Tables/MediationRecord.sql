﻿CREATE TABLE [Mediation_Generic].[MediationRecord] (
    [EventId]               INT            IDENTITY (1, 1) NOT NULL,
    [SessionId]             NVARCHAR (200) NULL,
    [EventTime]             DATETIME       NULL,
    [EventStatus]           TINYINT        NULL,
    [MediationDefinitionId] INT            NULL,
    [EventDetails]          NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_StoreStagingRecord] PRIMARY KEY CLUSTERED ([EventId] ASC)
);

