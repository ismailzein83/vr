CREATE TABLE [TOneWhS_BE].[CustomerRecurringChargesType] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK_CustomerRecurringChargesType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

