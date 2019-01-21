CREATE TABLE [RA].[MobileNetwork] (
    [ID]               INT           IDENTITY (1, 1) NOT NULL,
    [MCCID]            INT           NULL,
    [CreatedBy]        INT           NULL,
    [CreatedTime]      DATETIME      NULL,
    [LastModifiedBy]   INT           NULL,
    [LastModifiedTime] DATETIME      NULL,
    [timestamp]        ROWVERSION    NULL,
    [NetworkName]      VARCHAR (200) NULL,
    [Settings]         VARCHAR (MAX) NULL,
    [MobileCountryID]  INT           NULL,
    CONSTRAINT [PK_MobileNetwork] PRIMARY KEY CLUSTERED ([ID] ASC)
);

