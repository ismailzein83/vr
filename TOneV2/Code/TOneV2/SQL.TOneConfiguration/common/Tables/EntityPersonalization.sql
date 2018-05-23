CREATE TABLE [common].[EntityPersonalization] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserID]           INT            NULL,
    [EntityUniqueName] VARCHAR (1000) NOT NULL,
    [Details]          NVARCHAR (MAX) NOT NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_EntityPersonalization_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       CONSTRAINT [DF_EntityPersonalization_LastModifiedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT            NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK_EntityPersonalization] PRIMARY KEY CLUSTERED ([ID] ASC)
);

