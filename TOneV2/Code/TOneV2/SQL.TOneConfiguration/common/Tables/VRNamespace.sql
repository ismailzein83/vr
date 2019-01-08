CREATE TABLE [common].[VRNamespace] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             VARCHAR (255)    NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_VRNamespace_LastModifiedTime] DEFAULT (getdate()) NULL,
    [CreatedTime]      DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_VRNamespace] PRIMARY KEY CLUSTERED ([ID] ASC)
);





