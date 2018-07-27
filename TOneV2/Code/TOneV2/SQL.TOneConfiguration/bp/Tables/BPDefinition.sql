CREATE TABLE [bp].[BPDefinition] (
    [ID]           UNIQUEIDENTIFIER CONSTRAINT [DF__BPDefinition__ID__14DBF883] DEFAULT (newid()) NOT NULL,
    [Name]         VARCHAR (255)    NOT NULL,
    [Title]        NVARCHAR (255)   NOT NULL,
    [FQTN]         VARCHAR (1000)   NULL,
    [VRWorkflowId] UNIQUEIDENTIFIER NULL,
    [Config]       NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]  DATETIME         CONSTRAINT [DF_BPDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]    ROWVERSION       NULL,
    CONSTRAINT [pk_BPDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);









