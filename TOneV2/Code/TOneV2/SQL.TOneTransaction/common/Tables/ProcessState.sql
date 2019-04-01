CREATE TABLE [common].[ProcessState] (
    [UniqueName]       VARCHAR (255)  NOT NULL,
    [Settings]         NVARCHAR (MAX) NOT NULL,
    [LastModifiedTime] DATETIME       CONSTRAINT [DF_ProcessState_LastModifiedTime] DEFAULT (getdate()) NOT NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_ProcessState_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK_ProcessState] PRIMARY KEY CLUSTERED ([UniqueName] ASC)
);







