CREATE TABLE [NIM].[Path] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    CONSTRAINT [PK__Path__3214EC2752442E1F] PRIMARY KEY CLUSTERED ([ID] ASC)
);

