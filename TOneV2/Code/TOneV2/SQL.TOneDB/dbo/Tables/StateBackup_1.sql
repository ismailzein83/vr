CREATE TABLE [dbo].[StateBackup] (
    [ID]                      INT           IDENTITY (1, 1) NOT NULL,
    [StateBackupType]         TINYINT       CONSTRAINT [DF_StateBackup_StateBackupType] DEFAULT ((0)) NOT NULL,
    [CustomerID]              VARCHAR (10)  NULL,
    [SupplierID]              VARCHAR (10)  NULL,
    [Created]                 DATETIME      CONSTRAINT [DF_StateBackup_Created] DEFAULT (getdate()) NOT NULL,
    [Notes]                   TEXT          NULL,
    [StateData]               IMAGE         NOT NULL,
    [UserId]                  INT           NOT NULL,
    [timestamp]               ROWVERSION    NOT NULL,
    [RestoreDate]             DATETIME      NULL,
    [ResponsibleForRestoring] VARCHAR (200) NULL,
    CONSTRAINT [PK_StateChange] PRIMARY KEY CLUSTERED ([ID] ASC)
);

