CREATE TABLE [dbo].[Pop] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NCHAR (255)    NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Quantity]    INT            NULL,
    [Location]    NVARCHAR (MAX) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_Pop] PRIMARY KEY CLUSTERED ([ID] ASC)
);

