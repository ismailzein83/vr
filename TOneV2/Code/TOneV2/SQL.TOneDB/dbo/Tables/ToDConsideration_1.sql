CREATE TABLE [dbo].[ToDConsideration] (
    [ToDConsiderationID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [ZoneID]             INT            NOT NULL,
    [SupplierID]         VARCHAR (5)    NOT NULL,
    [CustomerID]         VARCHAR (5)    NOT NULL,
    [BeginTime]          VARCHAR (12)   NULL,
    [EndTime]            VARCHAR (12)   NULL,
    [WeekDay]            TINYINT        NULL,
    [HolidayDate]        SMALLDATETIME  NULL,
    [HolidayName]        NVARCHAR (255) NULL,
    [RateType]           TINYINT        NULL,
    [BeginEffectiveDate] SMALLDATETIME  NULL,
    [EndEffectiveDate]   SMALLDATETIME  NULL,
    [IsEffective]        AS             (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
    [IsActive]           AS             ([dbo].[IsToDActive]([HolidayDate],[WeekDay],[BeginTime],[EndTime],getdate())),
    [UserID]             INT            NULL,
    [timestamp]          ROWVERSION     NULL,
    CONSTRAINT [PK_ToDConsideration] PRIMARY KEY CLUSTERED ([ToDConsiderationID] ASC),
    CONSTRAINT [FK_ToDConsideration_CarrierAccount] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID]),
    CONSTRAINT [FK_ToDConsideration_CarrierAccount1] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
);

