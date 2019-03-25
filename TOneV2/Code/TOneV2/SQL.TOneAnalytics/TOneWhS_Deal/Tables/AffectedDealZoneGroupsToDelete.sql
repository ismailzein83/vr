﻿CREATE TABLE [TOneWhS_Deal].[AffectedDealZoneGroupsToDelete] (
    [DealID]           INT        NOT NULL,
    [ZoneGroupNb]      INT        NOT NULL,
    [IsSale]           BIT        NOT NULL,
    [CreatedTime]      DATETIME   CONSTRAINT [DF_AffectedDealZoneGroupsToDelete_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [LastModifiedTime] DATETIME   CONSTRAINT [DF_AffectedDealZoneGroupsToDelete_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION NULL
);



