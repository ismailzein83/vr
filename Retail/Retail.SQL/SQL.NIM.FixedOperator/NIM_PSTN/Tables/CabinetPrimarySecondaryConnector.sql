CREATE TABLE [NIM_PSTN].[CabinetPrimarySecondaryConnector] (
    [Id]               BIGINT     IDENTITY (1, 1) NOT NULL,
    [PrimaryPort]      BIGINT     NULL,
    [SecondaryPort]    BIGINT     NULL,
    [CreatedBy]        INT        NULL,
    [CreatedTime]      DATETIME   NULL,
    [LastModifiedBy]   INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

