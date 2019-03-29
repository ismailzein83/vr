CREATE TABLE [NetworkRentalManager].[DarkFiberRP] (
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)  NULL,
    [Capacity]         INT             NULL,
    [Price]            DECIMAL (20, 8) NULL,
    [Currency]         INT             NULL,
    [RecurringPeriod]  INT             NULL,
    [CreatedTime]      DATETIME        NULL,
    [CreatedBy]        INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    [timestamp]        ROWVERSION      NULL,
    CONSTRAINT [PK__DarkFibe__3214EC2747DBAE45] PRIMARY KEY CLUSTERED ([ID] ASC)
);

