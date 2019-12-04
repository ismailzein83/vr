CREATE TABLE [CRMFixedOper].[NetworkServiceType] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    [Command]          NVARCHAR (MAX)   NULL,
    [CancelCommand]    NVARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

