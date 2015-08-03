CREATE procedure [dbo].[GetSettlementInvoice]
    (@Id int)
AS

set nocount on

SELECT [Id]
      ,[IssueDate]
      ,[DueDate]
      ,[FromDate]
      ,[ToDate]
  FROM [SettlementInvoice]
  where Id = @Id