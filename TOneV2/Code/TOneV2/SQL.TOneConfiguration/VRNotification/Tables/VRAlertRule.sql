CREATE TABLE [VRNotification].[VRAlertRule] (
    [ID]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_VRAlertRule_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_VRAlertRule] PRIMARY KEY CLUSTERED ([ID] ASC)
);

