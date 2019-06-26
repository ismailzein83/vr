CREATE TABLE [NIM_PSTN].[MDFCabinetConnector] (
    [Id]                     BIGINT     IDENTITY (1, 1) NOT NULL,
    [MDFVerticalPort]        BIGINT     NULL,
    [CabinetPrimarySidePort] BIGINT     NULL,
    [CreatedBy]              INT        NULL,
    [CreatedTime]            DATETIME   NULL,
    [LastModifiedBy]         INT        NULL,
    [LastModifiedTime]       DATETIME   NULL,
    [timestamp]              ROWVERSION NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_MDFCabinetConnector_CabinetPort] UNIQUE NONCLUSTERED ([CabinetPrimarySidePort] ASC),
    CONSTRAINT [IX_MDFCabinetConnector_VerticalPort] UNIQUE NONCLUSTERED ([MDFVerticalPort] ASC)
);



