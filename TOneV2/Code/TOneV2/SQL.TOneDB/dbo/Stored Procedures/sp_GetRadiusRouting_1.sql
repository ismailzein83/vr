-- =============================================
-- Author:		<Vanrise, Rabih Fashwal>
-- Create date: <2015-01-22>
-- Description:	<Get the Routing for MVTS Radius>
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetRadiusRouting]
	-- Add the parameters for the stored procedure here
@SwitchID INT,
@MaxSuppliersNumber INT,
@Customers VARCHAR(MAX),
@LastUpdated DATETIME
AS
BEGIN
	SET NOCOUNT ON;

IF(@LastUpdated IS NULL)
BEGIN
  ;with customers as ( select distinct Carrieraccountid, identifier from SwitchCarrierMapping where inroute='Y' and switchid=@SwitchID)
   , suppliers as ( select distinct Carrieraccountid,identifier from SwitchCarrierMapping where outroute='Y' and switchid=@SwitchID)
   , Supper as (select distinct Carrieraccountid as Carrieraccountid,count(*) as counts from SwitchCarrierMapping where outroute='Y' and switchid=@SwitchID
 group by  Carrieraccountid)
 	,RadiusRoutes AS (
			select m.identifier Customer
            ,  r.code,
            s.identifier routeoptions
            ,(ROW_NUMBER() OVER (PARTITION BY R.RouteID ORDER BY Ro.Priority desc ,Ro.SupplierActiveRate asc)) AS priority
            ,ro.Percentage
            ,case when ro.percentage=0 then 'N' when ro.percentage/sp.counts is null then 'N' else 'Y' end as IsPercentage
            from route r with (nolock) left join routeoption ro with (nolock) on r.routeid=ro.routeid and Ro.[State]=1
            inner join customers m on m.carrieraccountid = r.customerid 
            left join suppliers s on s.carrieraccountid = ro.supplierid 
            left join Supper sp on sp.carrieraccountid=s.carrieraccountid
            where 
  
                 CustomerID IN (SELECT pa.[value] FROM dbo.ParseArray(@Customers,',') pa)
                 --r.CustomerID = 'c001' and r.Code = '96134'
)

						
						SELECT  Customer, code , routeoptions, @MaxSuppliersNumber - priority + 1 AS [Priority], percentage, ispercentage
						FROM RadiusRoutes WHERE priority <= @MaxSuppliersNumber ORDER  BY customer,code, priority DESC
END


else

begin
  ;with customers as ( select distinct Carrieraccountid, identifier from SwitchCarrierMapping where inroute='Y' and switchid=@SwitchID)
   , suppliers as ( select distinct Carrieraccountid,identifier from SwitchCarrierMapping where outroute='Y' and switchid=@SwitchID)
   , Supper as (select distinct Carrieraccountid as Carrieraccountid,count(*) as counts from SwitchCarrierMapping where outroute='Y' and switchid=@SwitchID
 group by  Carrieraccountid)
 	,RadiusRoutes AS (
			select m.identifier Customer
            ,  r.code,
            s.identifier routeoptions
            ,(ROW_NUMBER() OVER (PARTITION BY R.RouteID ORDER BY Ro.Priority desc ,Ro.SupplierActiveRate asc)) AS priority
            ,ro.Percentage
            ,case when ro.percentage=0 then 'N' when ro.percentage/sp.counts is null then 'N' else 'Y' end as IsPercentage
            from route r with (nolock) left join routeoption ro with (nolock) on r.routeid=ro.routeid and Ro.[State]=1
            inner join customers m on m.carrieraccountid = r.customerid 
            left join suppliers s on s.carrieraccountid = ro.supplierid 
            left join Supper sp on sp.carrieraccountid=s.carrieraccountid
            WHERE r.updated > @LastUpdated 
)

						
						SELECT  Customer, code , routeoptions, @MaxSuppliersNumber - priority + 1 AS [Priority], percentage, ispercentage
						FROM RadiusRoutes WHERE priority <= @MaxSuppliersNumber ORDER  BY customer,code, priority DESC

end
end