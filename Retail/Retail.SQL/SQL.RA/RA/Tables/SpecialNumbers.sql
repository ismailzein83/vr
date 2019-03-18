CREATE TABLE [RA].[SpecialNumbers] (
    [ID]               BIGINT         NOT NULL,
    [GroupName]        VARCHAR (MAX)  NOT NULL,
    [Settings]         NVARCHAR (MAX) NOT NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [timestamp]        ROWVERSION     NOT NULL,
    CONSTRAINT [PK__SpecialN__3214EC2772C60C4A] PRIMARY KEY CLUSTERED ([ID] ASC)
);

