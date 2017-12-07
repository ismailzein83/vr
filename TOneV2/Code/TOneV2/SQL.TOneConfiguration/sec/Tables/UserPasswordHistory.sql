CREATE TABLE [sec].[UserPasswordHistory] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserID]           INT            NOT NULL,
    [Password]         NVARCHAR (255) NULL,
    [IsChangedByAdmin] BIT            NOT NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_UserPasswordHistory_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK_UserPasswordHistory] PRIMARY KEY CLUSTERED ([ID] ASC)
);

