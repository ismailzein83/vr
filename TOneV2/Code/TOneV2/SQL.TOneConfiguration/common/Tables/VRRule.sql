CREATE TABLE [common].[VRRule] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [RuleDefinitionId] UNIQUEIDENTIFIER NOT NULL,
    [Settings]         NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_VRRule_CreatedTime] DEFAULT (getdate()) NULL,
    [IsDeleted]        BIT              CONSTRAINT [DF_VRRule_IsDeleted] DEFAULT ((0)) NULL,
    [timestamp]        ROWVERSION       NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_VRRule_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_VRRule] PRIMARY KEY CLUSTERED ([ID] ASC)
);





