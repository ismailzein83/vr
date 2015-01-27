﻿CREATE TABLE [dbo].[SwitchReleaseCode] (
    [SwitchID]    TINYINT        CONSTRAINT [DF_SwitchReleaseCode_SwitchID] DEFAULT ((0)) NOT NULL,
    [ReleaseCode] VARCHAR (50)   NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [IsDelivered] CHAR (1)       CONSTRAINT [DF_SwitchReleaseCode_IsDelivered] DEFAULT ('N') NOT NULL,
    [IsoCode]     VARCHAR (50)   NULL,
    CONSTRAINT [PK_SwitchReleaseCode] PRIMARY KEY CLUSTERED ([SwitchID] ASC, [ReleaseCode] ASC)
);

