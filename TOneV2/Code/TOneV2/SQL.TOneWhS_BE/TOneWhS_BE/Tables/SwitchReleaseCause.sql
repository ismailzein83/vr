﻿CREATE TABLE [TOneWhS_BE].[SwitchReleaseCause] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [SwitchID]    INT            NULL,
    [ReleaseCode] NVARCHAR (255) NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_SwitchReleaseCause_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_SwitchReleaseCause] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_SwitchReleaseCause] UNIQUE NONCLUSTERED ([ReleaseCode] ASC, [SwitchID] ASC)
);

