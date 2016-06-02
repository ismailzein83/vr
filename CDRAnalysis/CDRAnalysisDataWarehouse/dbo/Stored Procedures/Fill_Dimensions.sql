
CREATE PROCEDURE [dbo].[Fill_Dimensions]

AS


--=============== filling Dimensions =============

----====== Dim_CallClass

INSERT INTO [dbo].[Dim_CallClass] ([Pk_CallClassId],[Name]) 
SELECT [Id] ,[Description] FROM [CDRAnalysis].[FraudAnalysis].[CallClass]

--INSERT INTO [dbo].[Dim_CallClass] ([Pk_CallClassId],[Name]) values (-99,'N/A')

--====== Dim_CallType

--INSERT INTO [dbo].[Dim_CallType]([Pk_CallTypeId],[Name])
--SELECT [Code] ,[Description]  FROM [CDRAnalysis].[FraudAnalysis].[CallType]


INSERT INTO [dbo].[Dim_CallType]([Pk_CallTypeId],[Name]) values (1,'outgoing Voice')
INSERT INTO [dbo].[Dim_CallType]([Pk_CallTypeId],[Name]) values (2,'Incoming Voice')
INSERT INTO [dbo].[Dim_CallType]([Pk_CallTypeId],[Name]) values (29,'call Forward')
INSERT INTO [dbo].[Dim_CallType]([Pk_CallTypeId],[Name]) values (30,'Incoming Sms')
INSERT INTO [dbo].[Dim_CallType]([Pk_CallTypeId],[Name]) values (31,'Outgoing Sms')
INSERT INTO [dbo].[Dim_CallType]([Pk_CallTypeId],[Name]) values (26,'Roaming call forward')


--INSERT INTO [dbo].[Dim_CallType]([Pk_CallTypeId],[Name]) values (-99,'N/A')





--====== Dim_CaseStatus
INSERT INTO [dbo].[Dim_CaseStatus]([Pk_CaseStatusId]
           ,[Name])SELECT [Id],[Name] FROM [CDRAnalysis].[FraudAnalysis].[CaseStatus]
--INSERT INTO [dbo].[Dim_CaseStatus]([Pk_CaseStatusId]
--           ,[Name])values (-99,'N/A')
      
   
--====== dbo.Dim_AccountStatus 
INSERT INTO dbo.Dim_AccountStatus([Pk_AccountStatusId],[Name])
SELECT [Id],[Name] FROM [CDRAnalysis].[FraudAnalysis].[CaseStatus]
--INSERT INTO [dbo].[Dim_CaseStatus]([Pk_CaseStatusId]
--           ,[Name])values (-99,'N/A')   
      
      
           
--====== Dim_Filters
INSERT INTO [dbo].[Dim_Filters]([Pk_FilterId] ,[Name])
SELECT [Id],[Description]  FROM [CDRAnalysis].[FraudAnalysis].[Filter]
--INSERT INTO [dbo].[Dim_Filters]([Pk_FilterId] ,[Name])values (-99,'N/A')

--====== Dim_NetworkType
INSERT INTO [dbo].[Dim_NetworkType]([Pk_NetTypeId],[Name])VALUES(0,'Off Net')

INSERT INTO [dbo].[Dim_NetworkType]([Pk_NetTypeId],[Name])VALUES(1,'On Net')

INSERT INTO [dbo].[Dim_NetworkType]([Pk_NetTypeId],[Name])VALUES(2,'International')

INSERT INTO [dbo].[Dim_NetworkType]([Pk_NetTypeId],[Name])VALUES(3,'Unknown')

--INSERT INTO [dbo].[Dim_NetworkType]([Pk_NetTypeId],[Name])VALUEs (-99,'N/A')

--====== Dim_Period
INSERT INTO [dbo].[Dim_Period]([Pk_PeriodId] ,[Name])
SELECT [Id],[Description] FROM [CDRAnalysis].[FraudAnalysis].[Period]
--INSERT INTO [dbo].[Dim_Period]([Pk_PeriodId] ,[Name])values (-99,'N/A')


--====== Dim_StrategyKind

INSERT INTO [dbo].[Dim_StrategyKind]([PK_KindId] ,[Name]) VALUES (1,'System Built In')
INSERT INTO [dbo].[Dim_StrategyKind]([PK_KindId] ,[Name]) VALUES (2,'User Defined')
--INSERT INTO [dbo].[Dim_StrategyKind]([PK_KindId] ,[Name]) VALUES (-99,'N/A')

--====== Dim_SubscriberType
--INSERT INTO [dbo].[Dim_SubscriberType]([Pk_SubscriberTypeId],[Name])
--SELECT [Id],[Description]  FROM [CDRAnalysisDemo].[FraudAnalysis].[SubType]

--INSERT INTO [dbo].[Dim_SubscriberType]([Pk_SubscriberTypeId],[Name])VALUES (1,'Prepaid')
--INSERT INTO [dbo].[Dim_SubscriberType]([Pk_SubscriberTypeId],[Name])VALUES (2,'Postpaid')


INSERT INTO [dbo].[Dim_SubscriberType]([Name])
select distinct Sub_Type from [CDRAnalysis].[FraudAnalysis].[NormalCDR]
--INSERT INTO [dbo].[Dim_SubscriberType]([Name])VALUES ('N/A')



--====== Dim_SuspicionLevel
--INSERT INTO [dbo].[Dim_SuspicionLevel]([Pk_SuspicionLevelId],[Name])
--SELECT [Id],[Name] FROM [CDRAnalysisDemo].[FraudAnalysis].[SuspicionLevel]


INSERT INTO [dbo].[Dim_SuspicionLevel]([Pk_SuspicionLevelId],[Name])VALUES (1,'Clean')
INSERT INTO [dbo].[Dim_SuspicionLevel]([Pk_SuspicionLevelId],[Name])VALUES (2,'Suspicious')
INSERT INTO [dbo].[Dim_SuspicionLevel]([Pk_SuspicionLevelId],[Name])VALUES (3,'Highly Suspicious')
INSERT INTO [dbo].[Dim_SuspicionLevel]([Pk_SuspicionLevelId],[Name])VALUES (4,'Fraud')

--INSERT INTO [dbo].[Dim_SuspicionLevel]([Pk_SuspicionLevelId],[Name])VALUES (-99,'N/A')



--====== Dim_Users
INSERT INTO [dbo].[Dim_Users]([Pk_UserId],[Name])
SELECT [ID],[Name] FROM [CDRAnalysisConfigurationOnline].[sec].[User]
INSERT INTO [dbo].[Dim_Users]([Pk_UserId],[Name]) values (-99,'N/A')

--====== Dim_Strategy

INSERT INTO [dbo].[Dim_Strategy]
           (
            [Pk_StrategyId]
           ,[Name]
           ,[Type]
           ,[Kind]
           )
  SELECT st.[Id],st.[Name],p.[Description]
      , case when  st.isdefault=0 then 2 when   st.isdefault=1 then 1 end
  FROM [CDRAnalysis].[FraudAnalysis].[Strategy] st inner join [CDRAnalysis].FraudAnalysis.Period p  on  st.PeriodId=p.Id
  
  --INSERT INTO [dbo].[Dim_Strategy]
  --         (
  --          [Pk_StrategyId]
  --         ,[Name]
  --         ,[Type]
  --         ,[Kind]
  --         )
  --         VALUES (-99,'N/A','N/A','N/A')

--====== dbo.Dim_BTS
-- INSERT INTO [dbo].[Dim_BTS]
--           ([Pk_BTSId]
--           ,[Name])
--    SELECT distinct [BTS_Id],[BTS_Id]  FROM [CDRAnalysis].[FraudAnalysis].[NormalCDR] where [BTS_Id] is not null

----INSERT INTO [dbo].[Dim_BTS] ([Pk_BTSId],[Name]) values (0,'N/A')
    
--delete from dbo.Dim_BTS where Pk_BTSId is null
--====== Dim_Time


/*
Fill_Dimensions

select * from  dbo.Dim_Strategy
select * from  dbo.Dim_SuspicionLevel
select * from  dbo.Dim_SubscriberType
select * from  dbo.Dim_CallType
select * from  dbo.Dim_CallClass
select * from  dbo.Dim_CaseStatus
select * from  dbo.Dim_Period
select * from  dbo.Dim_NetworkType
select * from  dbo.Dim_Users
select * from  dbo.Dim_Filters
select * from  dbo.Dim_BTS

*/