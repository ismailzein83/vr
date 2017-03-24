CREATE VIEW Retail_EDR.vw_EDRs
AS
WITH AllEDRs AS (SELECT        IdCDR, StartDate, TrafficType, DirectionTraffic, Calling, Called, TypeNet, SourceOperator, DestinationOperator, SourceArea, DestinationArea, 
                                                                  TypeConsumed, Bag, PricePlan, Promotion, FileName, FileDate, CreationDate, Balance, Zone, Agent, AgentCommission, AccountID, 
                                                                  OriginatingZoneID, TerminatingZoneID, AirtimeRate, AirtimeAmount, TerminationRate, TerminationAmount, SaleRate, SaleAmount, Credit, MTRate, 
                                                                  MTAmount, Profit, TypeCalled
                                        FROM            Retail_EDR.Voice WITH (NOLOCK)
                                        UNION ALL
                                        SELECT        IdCDR, StartDate, TrafficType, DirectionTraffic, Calling, Called, TypeNet, SourceOperator, DestinationOperator, SourceArea, DestinationArea, 
                                                                 TypeConsumed, Bag, PricePlan, Promotion, FileName, FileDate, CreationDate, Balance, Zone, AgentID AS Agent, AgentCommission, AccountID, 
                                                                 OriginatingZoneID, TerminatingZoneID, AirtimeRate, AirtimeAmount, TerminationRate, TerminationAmount, SaleRate, SaleAmount, Credit, MTRate, 
                                                                 MTAmount, Profit, TypeMessage AS TypeCalled
                                        FROM            Retail_EDR.Message WITH (NOLOCK))
    SELECT        StartDate, TrafficType, DirectionTraffic, Calling, Called, TypeNet, SourceOperator, DestinationOperator, SourceArea, DestinationArea, TypeConsumed, Bag, 
                              PricePlan, Promotion, FileName, FileDate, CreationDate, Balance, Zone, Agent, AgentCommission, AccountID, OriginatingZoneID, TerminatingZoneID, AirtimeRate, 
                              AirtimeAmount, TerminationRate, TerminationAmount, SaleRate, SaleAmount, Credit, MTRate, MTAmount, Profit, TypeCalled, CONVERT(datetime, 
                              CONVERT(varchar(10), StartDate, 121)) AS Day, 'Week ' + RIGHT('00' + CONVERT(VARCHAR, DATEPART(wk, StartDate)), 2) + ' ' + CONVERT(VARCHAR, 
                              DATEPART(YYYY, StartDate)) AS Week, CONVERT(VARCHAR(7), StartDate, 120) AS Month
     FROM            AllEDRs AS AllEDRs_1
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'Retail_EDR', @level1type = N'VIEW', @level1name = N'vw_EDRs';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1[50] 4[25] 3) )"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2[66] 3) )"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2) )"
      End
      ActivePaneConfig = 5
   End
   Begin DiagramPane = 
      PaneHidden = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "AllEDRs_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 236
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 39
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1980
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      PaneHidden = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'Retail_EDR', @level1type = N'VIEW', @level1name = N'vw_EDRs';





