
create PROCEDURE [dbo].[ts_prNumberProfiling]
	
AS

truncate table NumberProfile

print 'outgoing'
---------------------------outgoing--------------------------------------------------------------------------------------
select a_temp SubscriberNumber , convert(varchar(12), connectDateTime , 102 ) Date_Day  ,  DATEPART(hour, connectdatetime ) Day_Hour  , 
sum( case when durationINseconds  = 0  then 0 else 1 end ) Count_out  , -1  Count_in , 
sum( case when durationINseconds  = 0  then 1 else 0 end ) Count_out_Fail, -1 Count_in_Fail ,
AVG (  case when durationINseconds  <> 0  then durationInseconds  else 0  end ) Call_out_Dur_AVG , -1 Call_in_Dur_Avg ,
SUM(durationinseconds/60) total_out_volume,-1 total_in_volume,
COUNT( distinct b_temp )  diff_output_numb_ , -1 diff_input_numbers ,
COUNT( distinct left(b_temp,4)) diff_dest_codes, -1 diff_sources_codes,
COUNT(distinct out_type)diff_out_type ,-1 diff_in_type
Into outgoing_t1
from normalCDR where isrepeated=0 
group by 

a_temp , convert(varchar(12), connectDateTime , 102 )  ,  DATEPART(hour, connectdatetime )


print 'profile out per day'
----------------------------------------------profile out per day-------------------------------------------
select a_temp SubscriberNumber , convert(varchar(12), connectDateTime , 102 ) Date_Day , 25 Day_Hour  , 
sum( case when durationINseconds  = 0  then 0 else 1 end ) Count_out  , -1  Count_in , 
sum( case when durationINseconds  = 0  then 1 else 0 end ) Count_out_Fail, -1 Count_in_Fail ,
AVG (  case when durationINseconds  <> 0  then durationInseconds  else 0  end ) Call_out_Dur_AVG , -1 Call_in_Dur_Avg ,
SUM(durationinseconds/60) total_out_volume,-1 total_in_volume,
COUNT( distinct b_temp )  diff_output_numb_ , -1 diff_input_numbers ,
COUNT( distinct left(b_temp,4)) diff_dest_codes, -1 diff_sources_codes,
COUNT(distinct out_type)diff_out_type ,-1 diff_in_type
Into out_t025
from normalCDR where isrepeated=0 
group by  a_temp  , convert(varchar(12), connectDateTime , 102 )   

 
print 'profile out per month'
-------------------------------------profile out per month----------------------------------

select a_temp SubscriberNumber , convert(varchar(6) , datepart( year,  connectDateTime ) * 100 + datepart( month,  connectDateTime )) Date_Day_1  ,   '2000-01-01' date_day, 30 Day_month  ,
sum( case when durationINseconds  = 0  then 0 else 1 end ) Count_out  , -1  Count_in , 
sum( case when durationINseconds  = 0  then 1 else 0 end ) Count_out_Fail, -1 Count_in_Fail ,
AVG (  case when durationINseconds  <> 0  then durationInseconds  else 0  end ) Call_out_Dur_AVG , -1 Call_in_Dur_Avg ,
SUM(durationinseconds/60) total_out_volume,-1 total_in_volume,
COUNT( distinct b_temp )  diff_output_numb_ , -1 diff_input_numbers ,
COUNT( distinct left(b_temp,4)) diff_dest_codes, -1 diff_sources_codes,
COUNT(distinct out_type)diff_out_type ,-1 diff_in_type
Into T_OUT_30
from normalCDR where  isrepeated=0 
group by 
a_temp , convert(varchar(6) , datepart( year,  connectDateTime ) * 100 + datepart( month,  connectDateTime ))



update T_OUT_30

set date_day = SUBSTRING(date_day_1,1,4)+'.'+SUBSTRING(date_day_1,5,2)+'.'+'01'

alter table T_OUT_30 drop  column date_day_1


print 'CDR hours out'
-------------------------------CDR hours out-------------------------------

insert into dbo.NumberProfile select * from outgoing_t1 
union
select * from T_OUT_30
union
select * from out_t025

order by date_day


print 'Incoming'
---------------------------------------------------Incoming----------------------

select b_temp SubscriberNumber , convert(varchar(12), ConnectDateTime , 102 ) Date_Day  ,  DATEPART(hour, Connectdatetime ) Day_Hour  , 
sum( case when durationINseconds  = 0  then 0 else 1 end ) Count_in  , -1  Count_out , 
sum( case when durationINseconds  = 0  then 1 else 0 end ) Count_In_Fail, -1 Count_out_Fail ,
AVG (  case when durationINseconds  <> 0  then durationInseconds  else 0  end ) Call_in_Dur_AVG , -1 Call_out_Dur_Avg ,
SUM(durationinseconds/60) total_in_volume,-1 total_out_volume,
COUNT( distinct a_temp)  diff_input_numbers , -1 diff_output_numb_, 
COUNT( distinct left(a_temp,4)) diff_sources_codes,-1 diff_dest_codes, 

COUNT(distinct in_type)diff_in_type,-1 diff_out_type
Into incoming_t1
from normalCDR where  isrepeated=0 
group by 
b_temp , convert(varchar(12), connectDateTime , 102 )  ,  DATEPART(hour, connectdatetime )


print 'profile IN per day'
-------------------------profile IN per day-------------------------------------------------------------------------



select b_temp SubscriberNumber , convert(varchar(12), connectDateTime , 102 ) Date_Day , 25 Day_Hour  , 
sum( case when durationINseconds  = 0  then 0 else 1 end ) Count_in  , -1  Count_out , 
sum( case when durationINseconds  = 0  then 1 else 0 end ) Count_in_Fail, -1 Count_out_Fail ,
AVG (  case when durationINseconds  <> 0  then durationInseconds  else 0  end ) Call_in_Dur_AVG , -1 Call_out_Dur_Avg ,
SUM(durationinseconds/60) total_in_volume,-1 total_out_volume,
COUNT( distinct a_temp)  diff_input_numbers , -1 diff_output_numb_, 
COUNT( distinct left(a_temp,4)) diff_sources_codes,-1 diff_dest_codes, 

COUNT(distinct in_type)diff_in_type,-1 diff_out_type
Into in_t025
from normalCDR where  isrepeated=0 
group by  b_temp  , convert(varchar(12), connectDateTime , 102 )   


print 'profile IN per month'
------------------------------------------------profile IN per month--------------


select b_temp SubscriberNumber , convert(varchar(6) , datepart( year,  connectDateTime ) * 100 + datepart( month,  connectDateTime )) Date_Day_1  ,   '2000-01-01' date_day, 30 Day_month  ,
sum( case when durationINseconds  = 0  then 0 else 1 end ) Count_in  , -1  Count_out , 
sum( case when durationINseconds  = 0  then 1 else 0 end ) Count_in_Fail, -1 Count_out_Fail ,
AVG (  case when durationINseconds  <> 0  then durationInseconds  else 0  end ) Call_in_Dur_AVG , -1 Call_out_Dur_Avg ,
SUM(durationinseconds/60) total_in_volume,-1 total_out_volume,
COUNT( distinct a_temp)  diff_input_numbers , -1 diff_output_numb_, 
COUNT( distinct left(a_temp,4)) diff_sources_codes,-1 diff_dest_codes, 

COUNT(distinct in_type)diff_in_type,-1 diff_out_type
Into T_IN_30
from normalCDR where isrepeated=0 
group by 
b_temp , convert(varchar(6) , datepart( year,  connectDateTime ) * 100 + datepart( month,  connectDateTime ))


update T_IN_30

set date_day =  SUBSTRING(date_day_1,1,4)+'.'+SUBSTRING(date_day_1,5,2)+'.'+'01'


alter table T_IN_30 drop  column date_day_1



select * into profile_in from incoming_t1 
union
select * from T_IN_30
union
select * from in_t025

order by date_day


update NumberProfile set Count_In = I.Count_in ,
 Count_In_Fail = I.Count_in_fail ,
 Call_in_Dur_Avg= I.Call_in_Dur_Avg , diff_input_numbers = I.diff_input_numbers,
 total_in_volume=I.total_in_volume,
 diff_sources_codes=I.diff_sources_codes,
 diff_in_type=I.diff_in_type
 
from NumberProfile  O  inner  join  profile_in I
on 
O.SubscriberNumber =  I.SubscriberNumber and  O.date_day = I.date_day and  O.day_hour = I.day_hour 


print 'get those who have incoming unit of time that have no outgoing'
-- get those who have incoming unit of time that have no outgoing

select I.* 
into setup_T_2
from profile_in I left outer join NumberProfile  O  
on 
O.SubscriberNumber =  I.SubscriberNumber and  O.date_day = I.date_day and  O.day_hour = I.day_hour 
where o.SubscriberNumber is null and o.date_day is null



--exec sp_rename 'in_t025.diff_output_numbers ', 'diff_output_numb_', 'Column' 

-------------------------------------------------------------------------------------
--begin tran 
--update sys.columns set name = 'SubscriberNumber'
--from sys.columns c inner join sysobjects o 
--on o.id= c.object_id 
--where o.name ='X123' and c.name = 'SubscriberNumber'


insert into NumberProfile
(SubscriberNumber,Date_Day,Day_Hour,Count_out,Count_in,Count_out_Fail,Count_in_Fail,Call_out_Dur_AVG,Call_in_Dur_Avg,total_out_volume,total_in_volume,diff_output_numb_,diff_input_numbers,diff_dest_codes,diff_sources_codes,diff_out_type,diff_in_type)
select 
SubscriberNumber,Date_Day,Day_Hour,Count_out,Count_in,Count_out_Fail,Count_in_Fail,Call_out_Dur_AVG,Call_in_Dur_Avg,total_out_volume,total_in_volume,diff_output_numb_,diff_input_numbers,diff_dest_codes,diff_sources_codes,diff_out_type,diff_in_type
 from setup_t_2 
 
 
 
drop table outgoing_t1
drop table out_t025
drop table T_OUT_30
drop table incoming_t1
drop table in_t025
drop table T_IN_30
drop table setup_t_2
drop table profile_in

/*

db_profile

*/