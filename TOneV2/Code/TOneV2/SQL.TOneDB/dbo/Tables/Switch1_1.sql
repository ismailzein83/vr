CREATE TABLE [dbo].[Switch1] (
    [SwitchID]                  TINYINT       NOT NULL,
    [Symbol]                    VARCHAR (10)  NULL,
    [Name]                      VARCHAR (512) NULL,
    [Description]               TEXT          NULL,
    [Configuration]             NTEXT         NULL,
    [LastCDRImportTag]          VARCHAR (255) NULL,
    [LastImport]                DATETIME      NULL,
    [LastRouteUpdate]           DATETIME      NULL,
    [UserID]                    INT           NULL,
    [timestamp]                 ROWVERSION    NOT NULL,
    [Enable_CDR_Import]         CHAR (1)      NOT NULL,
    [Enable_Routing]            CHAR (1)      NOT NULL,
    [LastAttempt]               DATETIME      NULL,
    [NominalTrunkCapacityInE1s] INT           NULL,
    [NominalVoipCapacityInE1s]  INT           NULL
);

