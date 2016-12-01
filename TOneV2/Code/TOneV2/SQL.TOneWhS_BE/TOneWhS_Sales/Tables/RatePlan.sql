CREATE TABLE [TOneWhS_Sales].[RatePlan] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [OwnerType]   INT            NOT NULL,
    [OwnerID]     INT            NOT NULL,
    [Changes]     NVARCHAR (MAX) NOT NULL,
    [Status]      INT            NOT NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_RatePlan_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_RatePlan] PRIMARY KEY CLUSTERED ([ID] ASC)
);





