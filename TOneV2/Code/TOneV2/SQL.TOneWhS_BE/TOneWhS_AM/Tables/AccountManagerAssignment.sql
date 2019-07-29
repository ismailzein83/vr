CREATE TABLE [TOneWhS_AM].[AccountManagerAssignment] (
    [ID]               INT        IDENTITY (1, 1) NOT NULL,
    [AccountManagerId] INT        NULL,
    [CarrierAccountId] INT        NULL,
    [BED]              DATETIME   NULL,
    [EED]              DATETIME   NULL,
    [CreatedTime]      DATETIME   CONSTRAINT [DF_AccountManagerAssignment_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT        NULL,
    [LastModifiedBy]   INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    CONSTRAINT [PK__AccountM__3214EC2716A6A769] PRIMARY KEY CLUSTERED ([ID] ASC)
);

