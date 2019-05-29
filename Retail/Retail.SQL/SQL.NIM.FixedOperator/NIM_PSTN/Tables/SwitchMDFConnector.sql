CREATE TABLE [NIM_PSTN].[SwitchMDFConnector] (
    [Id]                BIGINT     IDENTITY (1, 1) NOT NULL,
    [MDFHorizontalPort] BIGINT     NULL,
    [Device]            BIGINT     NULL,
    [CreatedBy]         INT        NULL,
    [CreatedTime]       DATETIME   NULL,
    [LastModifiedBy]    INT        NULL,
    [LastModifiedTime]  DATETIME   NULL,
    [timestamp]         ROWVERSION NULL,
    CONSTRAINT [PK__SwitchMD__3214EC072180FB33] PRIMARY KEY CLUSTERED ([Id] ASC)
);

