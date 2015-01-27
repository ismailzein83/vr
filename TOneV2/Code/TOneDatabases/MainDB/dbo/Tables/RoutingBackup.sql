CREATE TABLE [dbo].[RoutingBackup] (
    [ID]                      INT            IDENTITY (1, 1) NOT NULL,
    [Created]                 DATETIME       CONSTRAINT [DF_RoutingBackup_Created] DEFAULT (getdate()) NOT NULL,
    [Notes]                   NTEXT          NULL,
    [StateData]               IMAGE          NOT NULL,
    [UserId]                  INT            NOT NULL,
    [timestamp]               ROWVERSION     NOT NULL,
    [RestoreDate]             DATETIME       NULL,
    [ResponsibleForRestoring] NVARCHAR (200) NULL,
    CONSTRAINT [PK_RoutingBackup] PRIMARY KEY CLUSTERED ([ID] ASC)
);

