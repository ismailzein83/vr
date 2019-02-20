CREATE TABLE [TOneWhS_SMSBE].[SMSSwitch] (
    [ID]               INT          IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (50) NULL,
    [CreatedTime]      DATETIME     NULL,
    [CreatedBy]        INT          NULL,
    [LastModifiedTime] DATETIME     NULL,
    [LastModifiedBy]   DATETIME     NULL,
    [timestamp]        ROWVERSION   NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

