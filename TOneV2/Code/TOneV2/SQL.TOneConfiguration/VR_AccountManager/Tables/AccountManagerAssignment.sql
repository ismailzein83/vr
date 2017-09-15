CREATE TABLE [VR_AccountManager].[AccountManagerAssignment] (
    [ID]                     BIGINT           IDENTITY (1, 1) NOT NULL,
    [AssignmentDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [AccountManagerID]       BIGINT           NOT NULL,
    [AccountID]              NVARCHAR (400)   NOT NULL,
    [Settings]               NVARCHAR (MAX)   NULL,
    [BED]                    DATETIME         NOT NULL,
    [EED]                    DATETIME         NULL,
    [CreatedTime]            DATETIME         CONSTRAINT [DF_AccountManagerAssignment_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]              ROWVERSION       NULL,
    CONSTRAINT [PK_AccountManagerAssignment] PRIMARY KEY CLUSTERED ([ID] ASC)
);

