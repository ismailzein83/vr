
CREATE PROCEDURE [dbo].[sp_GetRadiusRouting_Differential]
@SwitchID INT,
@MaxSuppliersNumber INT,
@LastUpdated DATETIME
AS
BEGIN

DECLARE @SwitchID_Local INT,
@MaxSuppliersNumber_Local INT,
@LastUpdated_Local DATETIME

SELECT  @SwitchID_Local = @SwitchID,
@MaxSuppliersNumber_Local = @MaxSuppliersNumber,
@LastUpdated_Local = @LastUpdated

declare @Route_sys_date datetime
select @Route_sys_date = datetimevalue from systemparameter where name ='sys_LastRouteSynch'
    SET NOCOUNT ON;

  ;with customers as ( select distinct Carrieraccountid, identifier from SwitchCarrierMapping where inroute='Y' and switchid=@SwitchID_Local)
   , suppliers as ( select distinct Carrieraccountid,identifier from SwitchCarrierMapping where outroute='Y' and switchid=@SwitchID_Local)
   , Supper as (select distinct Carrieraccountid as Carrieraccountid,count(*) as counts from SwitchCarrierMapping where outroute='Y' and switchid=@SwitchID_Local
 group by  Carrieraccountid)
     ,RadiusRoutes AS (
            select m.identifier Customer
            ,  r.code,
              case when s.identifier is Null then 'BLK' else s.identifier end   routeoptions
            ,(ROW_NUMBER() OVER (PARTITION BY R.RouteID ORDER BY Ro.Priority desc ,Ro.SupplierActiveRate asc)) AS priority
            ,ro.Percentage
            ,case when ro.percentage=0 then 'N' when ro.percentage/sp.counts is null then 'N' else 'Y' end as IsPercentage
            from route r with (nolock) left join routeoption ro with (nolock) on r.routeid=ro.routeid and Ro.[State]=1
            inner join customers m on m.carrieraccountid = r.customerid
            left join suppliers s on s.carrieraccountid = ro.supplierid
            left join Supper sp on sp.carrieraccountid=s.carrieraccountid
            WHERE r.updated > @Route_sys_date 
)
                        SELECT  Customer, code , routeoptions, (ROW_NUMBER() OVER (PARTITION BY Customer, code ORDER BY Priority desc)) AS priority, percentage, ispercentage
                        FROM RadiusRoutes WHERE  priority <= @MaxSuppliersNumber_Local ORDER  BY customer,code, priority DESC
end