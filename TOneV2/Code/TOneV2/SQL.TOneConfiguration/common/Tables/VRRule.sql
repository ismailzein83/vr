CREATE TABLE [common].[VRRule] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [RuleDefinitionId] UNIQUEIDENTIFIER NOT NULL,
    [Settings]         NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_VRRule_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_VRRule] PRIMARY KEY CLUSTERED ([ID] ASC)
);

