
CREATE PROCEDURE [dbo].[Populate_DW] 
	
AS



truncate table dbo.Fact_Calls
truncate table dbo.Fact_Cases 


delete from dbo.Dim_Time
delete from dbo.Dim_Strategy
delete from dbo.Dim_SuspicionLevel
delete from dbo.Dim_SubscriberType
delete from dbo.Dim_CallType
delete from dbo.Dim_CallClass
delete from dbo.Dim_CaseStatus
delete from dbo.Dim_Period
delete from dbo.Dim_NetworkType
delete from dbo.Dim_Users
delete from dbo.Dim_Filters
delete from dbo.Dim_Time
delete from dbo.Dim_StrategyKind
delete from dbo.Dim_BTS
delete from dbo.Dim_AccountStatus
delete from dbo.Dim_MSISDN
delete from dbo.Dim_IMEI
IF (OBJECT_ID('FK_Fact_Calls_Dim_Time', 'F') IS NOT NULL)
BEGIN
ALTER TABLE dbo.Fact_Calls DROP CONSTRAINT FK_Fact_Calls_Dim_Time
END
IF (OBJECT_ID('FK_Fact_Cases_Dim_Time', 'F') IS NOT NULL)
BEGIN
ALTER TABLE dbo.Fact_Cases DROP CONSTRAINT FK_Fact_Cases_Dim_Time
END


IF (OBJECT_ID('FK_Fact_Cases_Dim_NetworkType', 'F') IS NOT NULL)
BEGIN
ALTER TABLE dbo.Fact_Cases DROP CONSTRAINT FK_Fact_Cases_Dim_NetworkType
END

IF (OBJECT_ID('FK_Fact_Cases_Dim_BTS', 'F') IS NOT NULL)
BEGIN
ALTER TABLE dbo.Fact_Cases DROP CONSTRAINT FK_Fact_Cases_Dim_BTS
END

IF (OBJECT_ID('FK_Fact_Cases_Dim_MSISDN', 'F') IS NOT NULL)
BEGIN
ALTER TABLE dbo.Fact_Cases DROP CONSTRAINT FK_Fact_Cases_Dim_MSISDN
END
IF (OBJECT_ID('FK_Fact_Cases_Dim_IMEI', 'F') IS NOT NULL)
BEGIN
ALTER TABLE dbo.Fact_Cases DROP CONSTRAINT FK_Fact_Cases_Dim_IMEI
END



--ALTER TABLE dbo.Fact_Calls DROP CONSTRAINT FK_Fact_Calls_Dim_Users1
--ALTER TABLE dbo.Fact_Cases DROP CONSTRAINT FK_Fact_Cases_Dim_Users1


exec dbo.Fill_Dimensions 
exec dbo.FillFactCallsHourly  @FromDate ='03/14/2015',@ToDate ='03/15/2015' 
exec FillFactCallsDaily @FromDate ='03/14/2015',@ToDate ='03/15/2015' 
exec dbo.FillFact_Cases @FromDate = '2016-04-14 00:00:00.00',@ToDate ='2016-04-14 23:00:00.00'
exec dbo.Fill_DimTime

exec dbo.Fill_BTS
exec dbo.Fill_MSISDN
exec dbo.Fill_IMEI

exec(' 
ALTER TABLE [dbo].[Fact_Calls]  ADD  CONSTRAINT [FK_Fact_Calls_Dim_Time] FOREIGN KEY([FK_ConnectTime])
REFERENCES [dbo].[Dim_Time] ([DateInstance])
'
)
exec(' 
ALTER TABLE [dbo].[Fact_Cases]  ADD  CONSTRAINT [FK_Fact_Cases_Dim_Time] FOREIGN KEY([FK_CaseGenerationTime])
REFERENCES [dbo].[Dim_Time] ([DateInstance])
'
)
exec(' 
ALTER TABLE [dbo].[Fact_Cases]  ADD  CONSTRAINT [FK_Fact_Cases_Dim_MSISDN] FOREIGN KEY([FK_MSISDN])
REFERENCES [dbo].[Dim_MSISDN] ([MSISDN])
'
)
exec(' 
ALTER TABLE [dbo].[Fact_Cases]  ADD  CONSTRAINT [FK_Fact_Cases_Dim_NetworkType] FOREIGN KEY([FK_NetworkType])
REFERENCES [dbo].[Dim_NetworkType] ([Pk_NetTypeId])
'
)
exec(' 
ALTER TABLE [dbo].[Fact_Cases]  ADD  CONSTRAINT [FK_Fact_Cases_Dim_IMEI] FOREIGN KEY([FK_IMEI])
REFERENCES [dbo].[Dim_IMEI] ([IMEI])
'
)
--exec(' 
--ALTER TABLE [dbo].[Fact_Calls]  ADD  CONSTRAINT [FK_Fact_Calls_Dim_Users1] FOREIGN KEY([FK_CaseUser])
--REFERENCES [dbo].[Dim_Users] ([Pk_UserId])
--'
--)
--exec(' 
--ALTER TABLE [dbo].[Fact_Cases]  ADD  CONSTRAINT [FK_Fact_Cases_Dim_Users1] FOREIGN KEY([FK_CaseUser])
--REFERENCES [dbo].[Dim_Users] ([Pk_UserId])
--'
--)


/*
Populate_DW

*/