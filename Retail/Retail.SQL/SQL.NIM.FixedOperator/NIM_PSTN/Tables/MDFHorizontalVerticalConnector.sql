CREATE TABLE [NIM_PSTN].[MDFHorizontalVerticalConnector] (
    [Id]                BIGINT     IDENTITY (1, 1) NOT NULL,
    [MDFVerticalPort]   BIGINT     NULL,
    [MDFHorizontalPort] BIGINT     NULL,
    [CreatedBy]         INT        NULL,
    [CreatedTime]       DATETIME   NULL,
    [LastModifiedBy]    INT        NULL,
    [LastModifiedTime]  DATETIME   NULL,
    [timestamp]         ROWVERSION NULL,
    CONSTRAINT [PK__Horizont__3214EC072CF2ADDF] PRIMARY KEY CLUSTERED ([Id] ASC)
);

