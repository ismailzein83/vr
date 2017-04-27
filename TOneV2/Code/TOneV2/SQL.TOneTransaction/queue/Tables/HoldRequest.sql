CREATE TABLE [queue].[HoldRequest] (
    [ID]                        BIGINT           IDENTITY (1, 1) NOT NULL,
    [BPInstanceID]              BIGINT           NOT NULL,
    [ExecutionFlowDefinitionId] UNIQUEIDENTIFIER NOT NULL,
    [From]                      DATETIME         NOT NULL,
    [To]                        DATETIME         NOT NULL,
    [QueuesToHold]              NVARCHAR (MAX)   NULL,
    [QueuesToProcess]           NVARCHAR (MAX)   NULL,
    [Status]                    INT              NOT NULL,
    [CreatedTime]               DATETIME         CONSTRAINT [DF_HoldRequest_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [timestamp]                 ROWVERSION       NULL,
    CONSTRAINT [PK_HoldRequest] PRIMARY KEY CLUSTERED ([ID] ASC)
);

