CREATE TABLE [Retail_BE].[ReleaseCause] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [ReleaseCode]      NVARCHAR (255) NULL,
    [ReleaseCodeName]  NVARCHAR (255) NULL,
    [IsDelivered]      BIT            NULL,
    [Description]      NVARCHAR (MAX) NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_ReleaseCause_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       CONSTRAINT [DF_ReleaseCause_LastModifiedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT            NULL,
    [timestamp]        ROWVERSION     NULL
);

