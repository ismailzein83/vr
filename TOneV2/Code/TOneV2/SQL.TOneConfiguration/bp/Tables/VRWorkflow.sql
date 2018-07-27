CREATE TABLE [bp].[VRWorkflow] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             VARCHAR (255)    NOT NULL,
    [Title]            NVARCHAR (255)   NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_VRWorkflow_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_VRWorkflow_LastModifiedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_VRWorkflow] PRIMARY KEY CLUSTERED ([ID] ASC)
);

