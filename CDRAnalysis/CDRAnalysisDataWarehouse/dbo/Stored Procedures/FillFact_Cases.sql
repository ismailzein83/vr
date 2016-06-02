
CREATE PROCEDURE [dbo].[FillFact_Cases]
	
	@FromDate datetime,
	@ToDate datetime
	
AS

truncate table dbo.Fact_Cases

INSERT INTO dbo.Fact_Cases
           (
            [FK_IMEI],[FK_MSISDN],[MS_Duration],[FK_CallClass],[FK_CallType],[FK_NetworkType],[FK_SubscriberType],[FK_BTS],FK_AccountStatus
           )
select  distinct nc.IMEI
,nc.MSISDN,
(nc.DurationInSeconds/60)
,cc.Id,nc.Call_Type
,cc.NetType,
nc.Sub_Type,
isnull(nc.BTS_Id,0),
acs.[Status]

from CDRAnalysis.FraudAnalysis.NormalCDR nc inner join  CDRAnalysis.FraudAnalysis.CallClass cc on nc.call_class=cc.[Description]
inner join CDRAnalysis.FraudAnalysis.AccountStatus acs on nc.MSISDN=acs.AccountNumber 
left join CDRAnalysis.FraudAnalysis.AccountCase ac on nc.MSISDN=ac.AccountNumber

where ac.CreatedTime between @FromDate and @ToDate 

--======Update from FraudAnalysis.StrategyExecutionDetails
update fc
set fc.MS_CaseId=ac.ID,fc.fk_CaseGenerationTime=ac.CreatedTime,fc.FK_CaseStatus=ac.[Status],fc.FK_CaseUser=isnull(ac.UserId,-99)
from  Fact_Cases fc inner join CDRAnalysis.FraudAnalysis.AccountCase ac on fc.FK_MSISDN=ac.AccountNumber

update fc
set fc.FK_strategy= se.StrategyID,fc.FK_SuspicionLevel=cd.SuspicionLevelID,fc.FK_Period=se.PeriodID
from  Fact_Cases fc inner join CDRAnalysis.FraudAnalysis.StrategyExecutionDetails cd on fc.FK_MSISDN=cd.AccountNumber
inner join CDRAnalysis.FraudAnalysis.StrategyExecution se on cd.StrategyExecutionID=se.ID


update fc
set fc.FK_StrategyKind= (Case when  s.IsDefault=0 then 2 else 1 end ) ,fc.FK_StrategyUser=isnull(s.UserId,-99)
from  Fact_Cases fc inner join CDRAnalysis.FraudAnalysis.Strategy s on fc.FK_strategy=s.Id
update Fact_Cases set FK_CaseUser =-99 where FK_CaseUser is null


/*

[FillFact_Cases]
	
	@FromDate = '2015-11-19 14:00:00.00',
	@ToDate ='2015-11-19 15:00:00.00'

*/