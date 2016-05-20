CREATE TABLE [QM_BE].[ConnectorZoneInfo] (
    [ID]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [ConnectorType]   VARCHAR (50)   NOT NULL,
    [ConnectorZoneID] VARCHAR (50)   NOT NULL,
    [Codes]           NVARCHAR (MAX) NOT NULL,
    [CreatedTime]     DATETIME       CONSTRAINT [DF_ConnectorZoneInfo_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]       ROWVERSION     NULL,
    CONSTRAINT [PK_ConnectorZoneInfo] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_ConnectorZoneInfo_ConnectorTypeZoneID] UNIQUE NONCLUSTERED ([ConnectorType] ASC, [ConnectorZoneID] ASC)
);

