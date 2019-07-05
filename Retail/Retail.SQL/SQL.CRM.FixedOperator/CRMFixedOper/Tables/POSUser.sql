CREATE TABLE [CRMFixedOper].[POSUser] (
    [ID]               INT              IDENTITY (1, 1) NOT NULL,
    [User]             INT              NULL,
    [POS]              UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_POSUser] PRIMARY KEY CLUSTERED ([ID] ASC)
);



