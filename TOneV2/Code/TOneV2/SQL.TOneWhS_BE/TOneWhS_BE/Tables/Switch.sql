CREATE TABLE [TOneWhS_BE].[Switch] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (50)   NOT NULL,
    [Settings]         NVARCHAR (MAX) NULL,
    [timestamp]        ROWVERSION     NULL,
    [SourceID]         VARCHAR (50)   NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_Switch_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    CONSTRAINT [PK_Switch] PRIMARY KEY CLUSTERED ([ID] ASC)
);











