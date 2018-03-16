CREATE TABLE [VRNotification].[VRAlertRule] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [RuleTypeId]       UNIQUEIDENTIFIER NOT NULL,
    [UserId]           INT              NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_VRAlertRule_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    CONSTRAINT [PK_VRAlertRule] PRIMARY KEY CLUSTERED ([ID] ASC)
);







