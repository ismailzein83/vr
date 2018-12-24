CREATE TABLE [common].[VRNamespace] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             VARCHAR (255)    NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_VRNamespace_LastModifiedTime] DEFAULT (getdate()) NULL,
    [CreatedTime]      DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL
);



