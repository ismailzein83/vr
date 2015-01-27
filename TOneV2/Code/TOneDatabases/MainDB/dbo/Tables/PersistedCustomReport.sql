CREATE TABLE [dbo].[PersistedCustomReport] (
    [ID]                   INT            IDENTITY (1, 1) NOT NULL,
    [Name]                 NVARCHAR (512) NOT NULL,
    [Created]              DATETIME       CONSTRAINT [DF__Persisted__Creat__0D5AD24C] DEFAULT (getdate()) NOT NULL,
    [Updated]              DATETIME       NULL,
    [RequiredPermissionID] VARCHAR (50)   NULL,
    [UserID]               INT            NULL,
    [Csharp_Code]          NTEXT          NULL,
    [IsEncrypted]          CHAR (1)       CONSTRAINT [DF_PersistedCustomReport_IsEncrypted] DEFAULT ('Y') NOT NULL,
    [timestamp]            ROWVERSION     NOT NULL,
    CONSTRAINT [PK__PersistedCustomR__0C66AE13] PRIMARY KEY CLUSTERED ([ID] ASC)
);

