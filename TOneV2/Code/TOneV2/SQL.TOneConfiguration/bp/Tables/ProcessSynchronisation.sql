CREATE TABLE [bp].[ProcessSynchronisation] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [IsEnabled]        BIT              NOT NULL,
    [Settings]         NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_ProcessSynchronisation_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]        INT              NOT NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_ProcessSynchronisation_LastModifiedTime] DEFAULT (getdate()) NOT NULL,
    [LastModifiedBy]   INT              NOT NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_ProcessSynchronisation] PRIMARY KEY CLUSTERED ([ID] ASC)
);

