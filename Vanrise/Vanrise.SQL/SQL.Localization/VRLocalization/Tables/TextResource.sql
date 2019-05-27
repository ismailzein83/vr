CREATE TABLE [VRLocalization].[TextResource] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [ResourceKey]      VARCHAR (255)    NOT NULL,
    [ModuleID]         UNIQUEIDENTIFIER NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_TextResource_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_TextResource_LastModifiedTime] DEFAULT (getdate()) NULL,
    [DefaultValue]     VARCHAR (255)    NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    CONSTRAINT [PK_TextResource] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_TextResource_Module] FOREIGN KEY ([ModuleID]) REFERENCES [VRLocalization].[Module] ([ID]),
    CONSTRAINT [IX_TextResource_Key] UNIQUE NONCLUSTERED ([ResourceKey] ASC)
);

