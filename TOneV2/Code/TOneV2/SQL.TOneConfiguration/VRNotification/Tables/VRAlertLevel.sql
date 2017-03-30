CREATE TABLE [VRNotification].[VRAlertLevel] (
    [ID]                         UNIQUEIDENTIFIER NOT NULL,
    [Name]                       NVARCHAR (255)   NULL,
    [BusinessEntityDefinitionID] UNIQUEIDENTIFIER NULL,
    [Settings]                   NVARCHAR (MAX)   NULL,
    [CreatedTime]                DATETIME         CONSTRAINT [DF_AlertLevel_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                  ROWVERSION       NULL,
    CONSTRAINT [PK_AlertLevel] PRIMARY KEY CLUSTERED ([ID] ASC)
);

