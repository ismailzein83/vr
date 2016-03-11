CREATE TABLE [InterConnect_BE].[OperatorAccount] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Suffix]      NVARCHAR (255) NOT NULL,
    [ProfileID]   INT            NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_OperatorAccount_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_OperatorAccount] PRIMARY KEY CLUSTERED ([ID] ASC)
);

