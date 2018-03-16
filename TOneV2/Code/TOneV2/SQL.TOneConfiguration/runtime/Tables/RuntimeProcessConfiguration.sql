CREATE TABLE [runtime].[RuntimeProcessConfiguration] (
    [ID]                         UNIQUEIDENTIFIER NOT NULL,
    [RuntimeNodeConfigurationID] UNIQUEIDENTIFIER NOT NULL,
    [Name]                       NVARCHAR (255)   NOT NULL,
    [Settings]                   NVARCHAR (MAX)   NULL,
    [timestamp]                  ROWVERSION       NULL,
    [CreatedTime]                DATETIME         CONSTRAINT [DF_RuntimeProcessConfiguration_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]                  INT              NULL,
    [LastModifiedTime]           DATETIME         CONSTRAINT [DF_RuntimeProcessConfiguration_LastModifiedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]             INT              NULL,
    CONSTRAINT [PK_RuntimeProcessConfiguration] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_RuntimeProcessConfiguration_RuntimeNodeConfiguration] FOREIGN KEY ([RuntimeNodeConfigurationID]) REFERENCES [runtime].[RuntimeNodeConfiguration] ([ID]),
    CONSTRAINT [IX_RuntimeProcessConfiguration_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

