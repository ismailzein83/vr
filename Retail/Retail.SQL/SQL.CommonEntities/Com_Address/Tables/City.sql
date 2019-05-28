CREATE TABLE [Com_Address].[City] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Region]           INT            NULL,
    [Name]             NVARCHAR (255) NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [timestamp]        ROWVERSION     NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

