CREATE TABLE [dbo].[OperatorDeclaredInfo] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_OperatorDeclaredInfo_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_OperatorDeclaredInfo] PRIMARY KEY CLUSTERED ([ID] ASC)
);

