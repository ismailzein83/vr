CREATE TABLE [bp].[ValidationMessage] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID] BIGINT         NOT NULL,
    [ParentProcessID]   BIGINT         NULL,
    [TargetKey]         VARCHAR (50)   NOT NULL,
    [TargetType]        VARCHAR (50)   NOT NULL,
    [Severity]          INT            NOT NULL,
    [Message]           NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ValidationMessage] PRIMARY KEY CLUSTERED ([ID] ASC)
);

