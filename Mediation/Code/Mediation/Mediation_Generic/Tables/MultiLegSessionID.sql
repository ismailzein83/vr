CREATE TABLE [Mediation_Generic].[MultiLegSessionID] (
    [MediationDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [SessionID]             NVARCHAR (200)   NOT NULL,
    [LegID]                 NVARCHAR (200)   NOT NULL,
    [CreatedTime]           DATETIME         CONSTRAINT [DF_MultiLegSessionID_CreatedTime] DEFAULT (getdate()) NULL
);

