CREATE TABLE [Com_BankEntities].[Branch] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [BankId]           INT            NULL,
    [Name]             NVARCHAR (255) NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [Code]             NVARCHAR (255) NULL,
    [AddressNotes]     NVARCHAR (255) NULL,
    [Region]           INT            NULL,
    [City]             INT            NULL,
    [Town]             INT            NULL,
    [Street]           NVARCHAR (255) NULL,
    [Building]         NVARCHAR (255) NULL,
    CONSTRAINT [PK__Branch__3214EC271DE57479] PRIMARY KEY CLUSTERED ([ID] ASC)
);

