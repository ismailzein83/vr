CREATE TABLE [Retail].[Account] (
    [ID]                  BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (255)   NOT NULL,
    [OldTypeID]           INT              NULL,
    [TypeID]              UNIQUEIDENTIFIER NOT NULL,
    [Settings]            NVARCHAR (MAX)   NULL,
    [ParentID]            BIGINT           NULL,
    [StatusID]            UNIQUEIDENTIFIER NULL,
    [SourceID]            NVARCHAR (255)   NULL,
    [ExecutedActionsData] NVARCHAR (MAX)   NULL,
    [ExtendedSettings]    NVARCHAR (MAX)   NULL,
    [CreatedTime]         DATETIME         NULL,
    [timestamp]           ROWVERSION       NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([ID] ASC)
);













