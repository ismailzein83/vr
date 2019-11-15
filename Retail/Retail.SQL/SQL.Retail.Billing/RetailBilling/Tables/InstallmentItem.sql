CREATE TABLE [RetailBilling].[InstallmentItem] (
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [InstallmentID]    BIGINT          NULL,
    [ItemNumber]       INT             NULL,
    [Amount]           DECIMAL (26, 6) NULL,
    [ScheduleDate]     DATE            NULL,
    [CreatedTime]      DATETIME        NULL,
    [CreatedBy]        INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    CONSTRAINT [PK__Installm__3214EC2724285DB4] PRIMARY KEY CLUSTERED ([ID] ASC)
);

