CREATE TABLE [dbo].[ChangeTracker] (
    [CustomerID]  VARCHAR (5)   NOT NULL,
    [LastUpdated] SMALLDATETIME NULL,
    [Notes]       VARCHAR (500) NULL,
    [timestamp]   ROWVERSION    NULL,
    CONSTRAINT [PK_ChangeTracker_1] PRIMARY KEY CLUSTERED ([CustomerID] ASC)
);

