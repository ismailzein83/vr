CREATE TABLE [dbo].[Switch] (
    [SwitchID]                  TINYINT       IDENTITY (1, 1) NOT NULL,
    [Symbol]                    VARCHAR (10)  NULL,
    [Name]                      VARCHAR (512) NULL,
    [Description]               TEXT          NULL,
    [Configuration]             NTEXT         NULL,
    [LastCDRImportTag]          VARCHAR (255) NULL,
    [LastImport]                DATETIME      NULL,
    [LastRouteUpdate]           DATETIME      NULL,
    [UserID]                    INT           NULL,
    [timestamp]                 ROWVERSION    NULL,
    [Enable_CDR_Import]         CHAR (1)      CONSTRAINT [DF_Switch_Enable_CDR_Import] DEFAULT ('Y') NOT NULL,
    [Enable_Routing]            CHAR (1)      CONSTRAINT [DF_Switch_Enable_Routing] DEFAULT ('Y') NOT NULL,
    [LastAttempt]               DATETIME      NULL,
    [NominalTrunkCapacityInE1s] INT           CONSTRAINT [DF_Switch_NominalCapacityInE1s] DEFAULT ((64)) NULL,
    [NominalVoipCapacityInE1s]  INT           NULL,
    CONSTRAINT [PK_Switch] PRIMARY KEY CLUSTERED ([SwitchID] ASC)
);

