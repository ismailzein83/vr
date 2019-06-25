CREATE TABLE [NIM_PSTN].[OLTHorizontalVerticalConnector] (
    [Id]                BIGINT     IDENTITY (1, 1) NOT NULL,
    [OLTHorizontalPort] BIGINT     NULL,
    [OLTVerticalPort]   BIGINT     NULL,
    [CreatedBy]         INT        NULL,
    [CreatedTime]       DATETIME   NULL,
    [LastModifiedBy]    INT        NULL,
    [LastModifiedTime]  DATETIME   NULL,
    [timestamp]         ROWVERSION NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

