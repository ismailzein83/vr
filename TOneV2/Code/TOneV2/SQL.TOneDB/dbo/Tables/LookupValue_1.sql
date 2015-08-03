CREATE TABLE [dbo].[LookupValue] (
    [LookupValueID] INT            IDENTITY (1, 1) NOT NULL,
    [LookupTypeID]  INT            NOT NULL,
    [Value]         NVARCHAR (255) NOT NULL,
    [Ordinal]       INT            NULL,
    [timestamp]     ROWVERSION     NOT NULL,
    CONSTRAINT [PK_LookupValue] PRIMARY KEY CLUSTERED ([LookupValueID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_LookupValue]
    ON [dbo].[LookupValue]([LookupTypeID] ASC);

