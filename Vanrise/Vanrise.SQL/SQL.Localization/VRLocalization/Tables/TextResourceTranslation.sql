CREATE TABLE [VRLocalization].[TextResourceTranslation] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [TextResourceID]   UNIQUEIDENTIFIER NOT NULL,
    [LanguageID]       UNIQUEIDENTIFIER NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_TextResourceTranslation_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_TextResourceTranslation_LastModifiedTime] DEFAULT (getdate()) NULL,
    [Value]            NVARCHAR (MAX)   NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    CONSTRAINT [PK_TextResourceTranslation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_TextResourceTranslation_Language] FOREIGN KEY ([LanguageID]) REFERENCES [VRLocalization].[Language] ([ID]),
    CONSTRAINT [FK_TextResourceTranslation_TextResource] FOREIGN KEY ([TextResourceID]) REFERENCES [VRLocalization].[TextResource] ([ID]),
    CONSTRAINT [IX_TextResourceTranslation_ResourceIdLangId] UNIQUE NONCLUSTERED ([TextResourceID] ASC, [LanguageID] ASC)
);

