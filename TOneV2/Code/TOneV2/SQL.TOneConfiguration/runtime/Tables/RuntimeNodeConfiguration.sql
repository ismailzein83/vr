CREATE TABLE [runtime].[RuntimeNodeConfiguration] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_RuntimeNodeConfiguration_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_RuntimeNodeConfiguration_LastModifiedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT              NULL,
    CONSTRAINT [PK_RuntimeNodeConfiguration] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_RuntimeNodeConfiguration_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

