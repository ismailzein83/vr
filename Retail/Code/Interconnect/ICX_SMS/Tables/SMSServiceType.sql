CREATE TABLE [ICX_SMS].[SMSServiceType] (
    [ID]               INT           IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (40) NULL,
    [Symbol]           NVARCHAR (40) NULL,
    [CreatedTime]      DATETIME      NULL,
    [LastModifiedTime] DATETIME      NULL,
    [CreatedBy]        INT           NULL,
    [LastModifiedBy]   INT           NULL,
    [timestamp]        ROWVERSION    NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

