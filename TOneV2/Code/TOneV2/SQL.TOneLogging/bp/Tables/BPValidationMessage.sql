CREATE TABLE [bp].[BPValidationMessage] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID] BIGINT         NOT NULL,
    [ParentProcessID]   BIGINT         NULL,
    [TargetKey]         VARCHAR (900)  NULL,
    [TargetType]        VARCHAR (50)   NOT NULL,
    [Severity]          INT            NOT NULL,
    [Message]           NVARCHAR (MAX) NULL,
    CONSTRAINT [IX_BPValidationMessage_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);






GO
CREATE NONCLUSTERED INDEX [IX_BPValidationMessage_Severity]
    ON [bp].[BPValidationMessage]([Severity] ASC);


GO
CREATE CLUSTERED INDEX [IX_BPValidationMessage_ProcessInstance]
    ON [bp].[BPValidationMessage]([ProcessInstanceID] ASC);

