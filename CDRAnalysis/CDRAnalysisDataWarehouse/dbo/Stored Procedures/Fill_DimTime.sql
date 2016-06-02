
CREATE PROCEDURE [dbo].[Fill_DimTime]
AS

--truncate table dbo.Dim_Time
create table #temp ([date] datetime)
insert into #temp([date])
select  FK_ConnectTime from dbo.Fact_Calls  where FK_ConnectTime is not null
union
select FK_CaseGenerationTime from dbo.Fact_Cases  where FK_CaseGenerationTime is not null


INSERT INTO [dbo].[Dim_Time]([DateInstance]) select distinct [date] from #temp

update [Dim_Time] 
set 
 [Year]=YEAR([DateInstance])
,[Month]=month([DateInstance])
,[Week]=datepart(week,[DateInstance])
,[Day]=day([DateInstance])
,[Hour]=datepart(hour,[DateInstance])
,[MonthName]=datename(month,[DateInstance])
,[DayName]=datename(dw,[DateInstance])

/*

 Fill_DimTime
 
  select * from dbo.Dim_Time
 select * from dbo.Fact_Calls
*/