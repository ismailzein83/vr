CREATE TABLE [TOneWhS_Deal].[DealSaleZoneGroup] (
    [ID]                     INT        IDENTITY (1, 1) NOT NULL,
    [DealID]                 INT        NOT NULL,
    [Volume]                 INT        NULL,
    [RetroactiveFromGroupID] INT        NULL,
    [BED]                    DATETIME   NOT NULL,
    [EED]                    DATETIME   NULL,
    [CreatedTime]            DATETIME   CONSTRAINT [DF_DealSaleZoneGroup_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]              ROWVERSION NULL,
    CONSTRAINT [PK_DealSaleZoneGroup] PRIMARY KEY CLUSTERED ([ID] ASC)
);

