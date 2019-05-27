CREATE TABLE [VRLocalization].[Language] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [ParentLanguageID] UNIQUEIDENTIFIER NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_Language_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_Language_LastModifiedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    [IsRTL]            BIT              NULL,
    CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Language_Language] FOREIGN KEY ([ParentLanguageID]) REFERENCES [VRLocalization].[Language] ([ID]),
    CONSTRAINT [IX_Language_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

