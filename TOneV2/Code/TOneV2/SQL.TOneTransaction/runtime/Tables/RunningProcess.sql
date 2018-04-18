CREATE TABLE [runtime].[RunningProcess] (
    [ID]                    INT              IDENTITY (1, 1) NOT NULL,
    [RuntimeNodeID]         UNIQUEIDENTIFIER NULL,
    [RuntimeNodeInstanceID] UNIQUEIDENTIFIER NULL,
    [OSProcessID]           INT              NULL,
    [StartedTime]           DATETIME         NOT NULL,
    [AdditionalInfo]        NVARCHAR (MAX)   NULL,
    [timestamp]             ROWVERSION       NULL,
    CONSTRAINT [PK_RunningProcess] PRIMARY KEY CLUSTERED ([ID] ASC)
);







