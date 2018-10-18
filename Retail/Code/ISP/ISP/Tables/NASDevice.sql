CREATE TABLE [ISP].[NASDevice] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [IPAddress]        VARCHAR (50)   NULL,
    [Port]             INT            NULL,
    [Username]         NVARCHAR (50)  NULL,
    [Password]         NVARCHAR (50)  NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK_NASDevice] PRIMARY KEY CLUSTERED ([ID] ASC)
);

