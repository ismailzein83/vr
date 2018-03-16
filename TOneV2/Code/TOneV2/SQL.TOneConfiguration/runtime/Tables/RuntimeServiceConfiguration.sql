CREATE TABLE [runtime].[RuntimeServiceConfiguration] (
    [ID]                            UNIQUEIDENTIFIER NOT NULL,
    [RuntimeProcessConfigurationID] UNIQUEIDENTIFIER NOT NULL,
    [Name]                          NVARCHAR (255)   NOT NULL,
    [Settings]                      NVARCHAR (MAX)   NULL,
    [timestamp]                     ROWVERSION       NULL,
    [CreatedTime]                   DATETIME         CONSTRAINT [DF_RuntimeServiceConfiguration_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]                     INT              NULL,
    [LastModifiedTime]              DATETIME         CONSTRAINT [DF_RuntimeServiceConfiguration_LastModifiedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]                INT              NULL,
    CONSTRAINT [PK_RuntimeServiceConfiguration] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_RuntimeServiceConfiguration_RuntimeProcessConfiguration] FOREIGN KEY ([RuntimeProcessConfigurationID]) REFERENCES [runtime].[RuntimeProcessConfiguration] ([ID]),
    CONSTRAINT [IX_RuntimeServiceConfiguration_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

