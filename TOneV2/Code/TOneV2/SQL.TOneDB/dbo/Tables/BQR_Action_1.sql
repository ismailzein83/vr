CREATE TABLE [dbo].[BQR_Action] (
    [ID]                   INT           IDENTITY (1, 1) NOT NULL,
    [ThresholdID]          INT           NOT NULL,
    [IsReflected]          CHAR (1)      NOT NULL,
    [State]                VARCHAR (MAX) NULL,
    [DateTimeCreated]      DATETIME      NOT NULL,
    [DateTimeUpdated]      DATETIME      NOT NULL,
    [TargetSupplierZoneID] INT           NOT NULL,
    [timestamp]            ROWVERSION    NOT NULL,
    [IsConfirmed]          CHAR (1)      NOT NULL,
    [UserID]               INT           NULL,
    CONSTRAINT [PK_BQR_Action] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_BQR_Action_Threshold] FOREIGN KEY ([ThresholdID]) REFERENCES [dbo].[BQR_Threshold] ([ID])
);

