CREATE TABLE [NIM_PSTN].[SwitchPhoneNumber] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [LocalAreaCode]    BIGINT           NULL,
    [SwitchLAC]        BIGINT           NULL,
    [PhoneNumber]      NVARCHAR (255)   NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK__PhoneNum__3214EC075165187F] PRIMARY KEY CLUSTERED ([Id] ASC)
);

