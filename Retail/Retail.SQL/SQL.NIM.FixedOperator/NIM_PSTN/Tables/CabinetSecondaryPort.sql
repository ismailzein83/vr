CREATE TABLE [NIM_PSTN].[CabinetSecondaryPort] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [SecondarySide]    BIGINT           NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

