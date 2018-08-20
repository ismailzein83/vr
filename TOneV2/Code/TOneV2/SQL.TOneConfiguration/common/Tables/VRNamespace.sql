CREATE TABLE [common].[VRNamespace] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        VARCHAR (255)    NULL,
    [Settings]    NVARCHAR (MAX)   NULL,
    [CreatedTime] DATETIME         NULL,
    [timestamp]   ROWVERSION       NULL
);

