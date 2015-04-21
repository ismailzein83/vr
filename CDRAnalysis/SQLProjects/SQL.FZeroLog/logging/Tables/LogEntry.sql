CREATE TABLE [logging].[LogEntry] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [MachineNameId]     INT            NOT NULL,
    [ApplicationNameId] INT            NOT NULL,
    [AssemblyNameId]    INT            NOT NULL,
    [TypeNameId]        INT            NOT NULL,
    [MethodNameId]      INT            NOT NULL,
    [EntryType]         INT            NOT NULL,
    [Message]           NVARCHAR (MAX) NULL,
    [EventTime]         DATETIME       NULL,
    CONSTRAINT [PK_LogMessage] PRIMARY KEY CLUSTERED ([ID] ASC)
);

