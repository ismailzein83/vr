CREATE TABLE [VRLocalization].[Module] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_Module_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_Module_LastModifiedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_Module_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

