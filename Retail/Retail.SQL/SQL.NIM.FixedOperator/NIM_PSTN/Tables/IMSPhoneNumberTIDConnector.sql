CREATE TABLE [NIM_PSTN].[IMSPhoneNumberTIDConnector] (
    [Id]               BIGINT     IDENTITY (1, 1) NOT NULL,
    [TID]              BIGINT     NULL,
    [PhoneNumber]      BIGINT     NULL,
    [CreatedBy]        INT        NULL,
    [CreatedTime]      DATETIME   NULL,
    [LastModifiedBy]   INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    CONSTRAINT [PK__IMSPhone__3214EC074865BE2A] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_IMSPhoneNumberTIDConnector_PhoneNumber] UNIQUE NONCLUSTERED ([PhoneNumber] ASC),
    CONSTRAINT [IX_IMSPhoneNumberTIDConnector_TID] UNIQUE NONCLUSTERED ([TID] ASC)
);



