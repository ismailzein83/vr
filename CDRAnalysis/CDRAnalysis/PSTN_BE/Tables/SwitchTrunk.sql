﻿CREATE TABLE [PSTN_BE].[SwitchTrunk] (
    [ID]              INT            IDENTITY (1, 1) NOT NULL,
    [SwitchID]        INT            NOT NULL,
    [Symbol]          NVARCHAR (50)  NOT NULL,
    [Name]            NVARCHAR (255) NOT NULL,
    [Direction]       INT            NOT NULL,
    [Type]            INT            NOT NULL,
    [LinkedToTrunkID] INT            NULL,
    [timestamp]       ROWVERSION     NULL,
    CONSTRAINT [PK_SwitchTrunk] PRIMARY KEY CLUSTERED ([ID] ASC)
);









