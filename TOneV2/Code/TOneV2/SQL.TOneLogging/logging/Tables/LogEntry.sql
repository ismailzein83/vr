CREATE TABLE [logging].[LogEntry] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [MachineNameId]     INT            NOT NULL,
    [ApplicationNameId] INT            NOT NULL,
    [AssemblyNameId]    INT            NOT NULL,
    [TypeNameId]        INT            NOT NULL,
    [MethodNameId]      INT            NOT NULL,
    [EntryType]         INT            NOT NULL,
    [EventType]         INT            NULL,
    [Message]           NVARCHAR (MAX) NULL,
    [ExceptionDetail]   NVARCHAR (MAX) NULL,
    [EventTime]         DATETIME       NULL,
    CONSTRAINT [IX_LogEntry_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);








GO
CREATE CLUSTERED INDEX [IX_LogEntry_Time]
    ON [logging].[LogEntry]([EventTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LogEntry_EntryType]
    ON [logging].[LogEntry]([EntryType] ASC);

