CREATE TABLE [TOneWhS_Deal].[DealZoneRate] (
    [ID]          BIGINT          NOT NULL,
    [DealId]      INT             NOT NULL,
    [ZoneGroupNb] INT             NOT NULL,
    [IsSale]      BIT             NOT NULL,
    [TierNb]      INT             NOT NULL,
    [ZoneId]      BIGINT          NOT NULL,
    [Rate]        DECIMAL (20, 8) NOT NULL,
    [BED]         DATETIME        NOT NULL,
    [EED]         DATETIME        NULL,
    CONSTRAINT [PK_DealZoneRate] PRIMARY KEY CLUSTERED ([ID] ASC)
);

