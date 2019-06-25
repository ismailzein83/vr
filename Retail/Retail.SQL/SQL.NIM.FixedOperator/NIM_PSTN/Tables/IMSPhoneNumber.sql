CREATE TABLE [NIM_PSTN].[IMSPhoneNumber] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [PhoneNumber]      NVARCHAR (255)   NULL,
    [LocalAreaCode]    BIGINT           NULL,
    [IMSLAC]           BIGINT           NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [Category]         INT              NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK__IMSPhone__3214EC073CF40B7E] PRIMARY KEY CLUSTERED ([Id] ASC)
);

