CREATE TABLE [cloud].[CloudApplicationUser] (
    [ApplicationID] INT            NULL,
    [UserID]        INT            NULL,
    [Settings]      NVARCHAR (MAX) NULL,
    [CreatedTime]   DATETIME       NULL,
    [timestamp]     ROWVERSION     NULL
);

