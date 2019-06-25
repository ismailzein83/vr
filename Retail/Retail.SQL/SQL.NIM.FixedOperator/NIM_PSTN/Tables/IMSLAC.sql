CREATE TABLE [NIM_PSTN].[IMSLAC] (
    [Id]               BIGINT     IDENTITY (1, 1) NOT NULL,
    [IMS]              BIGINT     NULL,
    [LocalAreaCode]    BIGINT     NULL,
    [CreatedBy]        INT        NULL,
    [CreatedTime]      DATETIME   NULL,
    [LastModifiedBy]   INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

