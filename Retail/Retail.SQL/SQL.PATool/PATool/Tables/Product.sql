CREATE TABLE [PATool].[Product] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [timestamp]        ROWVERSION     NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

