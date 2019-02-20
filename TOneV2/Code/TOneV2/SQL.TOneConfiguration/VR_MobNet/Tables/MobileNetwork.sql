CREATE TABLE [VR_MobNet].[MobileNetwork] (
    [ID]               INT           IDENTITY (1, 1) NOT NULL,
    [MobileCountryID]  INT           NULL,
    [NetworkName]      VARCHAR (200) NULL,
    [Settings]         VARCHAR (MAX) NULL,
    [CreatedBy]        INT           NULL,
    [CreatedTime]      DATETIME      NULL,
    [LastModifiedBy]   INT           NULL,
    [LastModifiedTime] DATETIME      NULL,
    [timestamp]        ROWVERSION    NULL,
    CONSTRAINT [PK_MobileNetwork] PRIMARY KEY CLUSTERED ([ID] ASC)
);

