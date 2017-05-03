CREATE TABLE [Mediation_Generic].[MultiLegSessionID] (
    [ID]                    BIGINT           IDENTITY (1, 1) NOT NULL,
    [MediationDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [SessionID]             NVARCHAR (200)   NOT NULL,
    [LegID]                 NVARCHAR (200)   NOT NULL,
    [CreatedTime]           DATETIME         CONSTRAINT [DF_MultiLegSessionID_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_MultiLegSessionID] PRIMARY KEY CLUSTERED ([ID] ASC)
);



