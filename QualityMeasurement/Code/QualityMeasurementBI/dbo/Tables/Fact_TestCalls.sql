CREATE TABLE [dbo].[Fact_TestCalls] (
    [MS_TestCallId]     INT      NULL,
    [FK_User]           INT      NULL,
    [FK_Supplier]       INT      NULL,
    [FK_Country]        INT      NULL,
    [FK_Zone]           INT      NULL,
    [FK_CallTestStatus] INT      NULL,
    [FK_CallTestResult] INT      NULL,
    [FK_Schedule]       INT      NULL,
    [FK_CreationDate]   DATETIME NULL,
    CONSTRAINT [FK_Fact_TestCalls_Dim_CallTestResult] FOREIGN KEY ([FK_CallTestResult]) REFERENCES [dbo].[Dim_CallTestResult] ([pk_CallTestResultId]),
    CONSTRAINT [FK_Fact_TestCalls_Dim_CallTestStatus] FOREIGN KEY ([FK_CallTestStatus]) REFERENCES [dbo].[Dim_CallTestStatus] ([pk_CallTestStatusId]),
    CONSTRAINT [FK_Fact_TestCalls_Dim_Countries] FOREIGN KEY ([FK_Country]) REFERENCES [dbo].[Dim_Countries] ([Pk_CountryId]),
    CONSTRAINT [FK_Fact_TestCalls_Dim_Schedules] FOREIGN KEY ([FK_Schedule]) REFERENCES [dbo].[Dim_Schedules] ([Pk_ScheduleId]),
    CONSTRAINT [FK_Fact_TestCalls_Dim_Suppliers] FOREIGN KEY ([FK_Supplier]) REFERENCES [dbo].[Dim_Suppliers] ([Pk_SupplierId]),
    CONSTRAINT [FK_Fact_TestCalls_Dim_Time] FOREIGN KEY ([FK_CreationDate]) REFERENCES [dbo].[Dim_Time] ([DateInstance]),
    CONSTRAINT [FK_Fact_TestCalls_Dim_Users] FOREIGN KEY ([FK_User]) REFERENCES [dbo].[Dim_Users] ([Pk_UserId]),
    CONSTRAINT [FK_Fact_TestCalls_Dim_Zones] FOREIGN KEY ([FK_Zone]) REFERENCES [dbo].[Dim_Zones] ([Pk_ZoneId])
);

