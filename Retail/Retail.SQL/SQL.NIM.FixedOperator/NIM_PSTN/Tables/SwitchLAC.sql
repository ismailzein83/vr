CREATE TABLE [NIM_PSTN].[SwitchLAC] (
    [Id]               BIGINT     IDENTITY (1, 1) NOT NULL,
    [Switch]           BIGINT     NULL,
    [LocalAreaCode]    BIGINT     NULL,
    [CreatedBy]        INT        NULL,
    [CreatedTime]      DATETIME   NULL,
    [LastModifiedBy]   INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    CONSTRAINT [PK__SwitchLA__3214EC0769FBBC1F] PRIMARY KEY CLUSTERED ([Id] ASC)
);

