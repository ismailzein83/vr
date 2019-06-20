CREATE TABLE [RetailBilling].[ContractSubType_IntlInternetCapE1] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [UserType]         INT              NULL,
    [SpeedFrom]        INT              NULL,
    [SpeedTo]          INT              NULL,
    [NeedMoTDecision]  BIT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK__Contract__3214EC275812160E] PRIMARY KEY CLUSTERED ([ID] ASC)
);

