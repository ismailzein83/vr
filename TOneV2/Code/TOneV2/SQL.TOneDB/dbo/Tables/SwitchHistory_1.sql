CREATE TABLE [dbo].[SwitchHistory] (
    [Date]              DATETIME      NULL,
    [SwitchID]          TINYINT       NULL,
    [Symbol]            VARCHAR (10)  NULL,
    [Name]              VARCHAR (512) NULL,
    [Description]       TEXT          NULL,
    [Configuration]     NTEXT         NULL,
    [LastCDRImportTag]  VARCHAR (255) NULL,
    [LastImport]        DATETIME      NULL,
    [LastRouteUpdate]   DATETIME      NULL,
    [UserID]            INT           NULL,
    [Enable_CDR_Import] CHAR (1)      NOT NULL,
    [Enable_Routing]    CHAR (1)      NOT NULL,
    [LastAttempt]       DATETIME      NULL,
    [timestamp]         ROWVERSION    NOT NULL,
    [DS_ID_auto]        INT           IDENTITY (1, 1) NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_SwitchHistory]
    ON [dbo].[SwitchHistory]([Date] ASC, [SwitchID] ASC);

