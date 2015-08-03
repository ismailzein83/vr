CREATE TABLE [dbo].[LookupType] (
    [LookupTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (100) NOT NULL,
    [timestamp]    ROWVERSION    NOT NULL,
    CONSTRAINT [PK_LookupType] PRIMARY KEY CLUSTERED ([LookupTypeID] ASC)
);

