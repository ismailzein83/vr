
CREATE PROCEDURE [dbo].[Fill_MSISDN]
	
AS

insert into dbo.Dim_MSISDN
select distinct FK_MSISDN from dbo.Fact_Cases
/* 

Fill_MSISDN
select * from Fact_Cases



 */