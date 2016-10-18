CREATE TABLE [bp].[BPTask] (
    [ID]                       BIGINT           IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID]        BIGINT           NOT NULL,
    [TypeID]                   UNIQUEIDENTIFIER NOT NULL,
    [OldTypeID]                INT              NULL,
    [Title]                    NVARCHAR (255)   NULL,
    [AssignedUsers]            VARCHAR (MAX)    NOT NULL,
    [AssignedUsersDescription] NVARCHAR (MAX)   NOT NULL,
    [ExecutedBy]               INT              NULL,
    [Status]                   INT              NOT NULL,
    [TaskData]                 NVARCHAR (MAX)   NULL,
    [TaskExecutionInformation] VARCHAR (MAX)    NULL,
    [Notes]                    NVARCHAR (MAX)   NULL,
    [Decision]                 NVARCHAR (255)   NULL,
    [CreatedTime]              DATETIME         CONSTRAINT [DF_BPTask_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedTime]          DATETIME         NOT NULL,
    [timestamp]                ROWVERSION       NULL,
    CONSTRAINT [PK_BPTask] PRIMARY KEY CLUSTERED ([ID] ASC)
);







