CREATE TABLE [common].[VRExclusiveSession] (
    [ID]                  INT              IDENTITY (1, 1) NOT NULL,
    [SessionTypeId]       UNIQUEIDENTIFIER NOT NULL,
    [TargetId]            NVARCHAR (400)   NOT NULL,
    [TakenByUserId]       INT              NULL,
    [LastTakenUpdateTime] DATETIME         NULL,
    [CreatedTime]         DATETIME         CONSTRAINT [DF_VRExclusiveSession_CreatedTime] DEFAULT (getdate()) NULL,
    [TakenTime]           DATETIME         NULL,
    [timestamp]           ROWVERSION       NULL,
    CONSTRAINT [PK_VRExclusiveSession] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_VRExclusiveSession_SessionTypeTarget] UNIQUE NONCLUSTERED ([SessionTypeId] ASC, [TargetId] ASC)
);



