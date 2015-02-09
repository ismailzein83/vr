CREATE TABLE [bp].[BPDefinition] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (255)  NOT NULL,
    [Title]       NVARCHAR (255) NOT NULL,
    [FQTN]        VARCHAR (1000) NOT NULL,
    [Config]      NVARCHAR (MAX) NOT NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_BPDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_BPDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BPDefinition_Name]
    ON [bp].[BPDefinition]([Name] ASC);

