CREATE TABLE [NIM_PSTN].[CabinetDPConnector] (
    [Id]                   BIGINT     IDENTITY (1, 1) NOT NULL,
    [CabinetSecondaryPort] BIGINT     NULL,
    [DPPort]               BIGINT     NULL,
    [CreatedBy]            INT        NULL,
    [CreatedTime]          DATETIME   NULL,
    [LastModifiedBy]       INT        NULL,
    [LastModifiedTime]     DATETIME   NULL,
    [timestamp]            ROWVERSION NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

