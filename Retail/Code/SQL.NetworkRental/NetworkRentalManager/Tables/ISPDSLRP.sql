CREATE TABLE [NetworkRentalManager].[ISPDSLRP] (
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)  NULL,
    [RangeStart]       INT             NULL,
    [RangeEnd]         INT             NULL,
    [Price]            DECIMAL (20, 6) NULL,
    [Currency]         INT             NULL,
    [RecurringPeriod]  INT             NULL,
    [CreatedTime]      DATETIME        NULL,
    [CreatedBy]        INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    [timestamp]        ROWVERSION      NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

