CREATE TABLE [RA].[SpecialNumbers] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [GroupName]        VARCHAR (MAX)  NOT NULL,
    [Settings]         NVARCHAR (MAX) NOT NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [timestamp]        ROWVERSION     NOT NULL,
    CONSTRAINT [PK_SpecialNumbers] PRIMARY KEY CLUSTERED ([ID] ASC)
);

