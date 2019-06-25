CREATE TABLE [Com_BankEntities].[Bank] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [Region]           INT            NULL,
    [City]             INT            NULL,
    [Town]             INT            NULL,
    [Building]         NVARCHAR (255) NULL,
    [AddressNotes]     NVARCHAR (255) NULL,
    [Code]             NVARCHAR (255) NULL,
    [Street]           NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

