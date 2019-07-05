CREATE TABLE [CRMFixedOper].[POS] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Region]           INT              NULL,
    [City]             INT              NULL,
    [Town]             INT              NULL,
    [Street]           NVARCHAR (255)   NULL,
    [Building]         NVARCHAR (255)   NULL,
    [AddressNotes]     NVARCHAR (255)   NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);



