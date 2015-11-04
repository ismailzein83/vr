CREATE TABLE [TOneWhS_Sales].[RatePlan] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [OwnerType] INT            NOT NULL,
    [OwnerID]   INT            NOT NULL,
    [Details]   NVARCHAR (MAX) NOT NULL,
    [Status]    NCHAR (10)     NULL,
    CONSTRAINT [PK_RatePlan] PRIMARY KEY CLUSTERED ([ID] ASC)
);

