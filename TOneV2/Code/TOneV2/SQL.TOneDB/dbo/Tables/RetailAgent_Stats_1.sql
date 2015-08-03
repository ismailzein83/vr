CREATE TABLE [dbo].[RetailAgent_Stats] (
    [ID]                INT             IDENTITY (1, 1) NOT NULL,
    [Date]              SMALLDATETIME   NOT NULL,
    [AgentID]           INT             NOT NULL,
    [SaleAmount]        FLOAT (53)      NULL,
    [CostAmount]        FLOAT (53)      NULL,
    [NumberOfCalls]     INT             NULL,
    [DurationInMinutes] NUMERIC (13, 4) NULL,
    [UnPricedCalls]     INT             NULL,
    [PricedCalls]       INT             NULL,
    [timestamp]         ROWVERSION      NOT NULL,
    CONSTRAINT [PK_RetailAgent_Stats] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_RetailAgent_Stats_date]
    ON [dbo].[RetailAgent_Stats]([Date] DESC);

