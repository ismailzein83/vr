CREATE TABLE [bp].[BPTask] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID] BIGINT         NOT NULL,
    [TypeID]            INT            NOT NULL,
    [Title]             NVARCHAR (255) NULL,
    [ExecutedBy]        INT            NULL,
    [Status]            INT            NOT NULL,
    [TaskInformation]   NVARCHAR (MAX) NULL,
    [CreatedTime]       DATETIME       CONSTRAINT [DF_BPTask_CreatedTime] DEFAULT (getdate()) NULL,
    [LastUpdatedTime]   DATETIME       NULL,
    [timestamp]         ROWVERSION     NULL,
    CONSTRAINT [PK_BPTask] PRIMARY KEY CLUSTERED ([ID] ASC)
);

