CREATE VIEW dbo.vw_Ana_AfghCDRs
AS
SELECT        r.CLI, r.b_number, r.AttemptDateTime, CASE WHEN r.[OriginationNetwork] IS NULL OR
                         r.[OriginationNetwork] = '' THEN 'INTERNATIONAL' ELSE r.[OriginationNetwork] END AS OriginationNetwork, CASE WHEN r.[OriginationNetwork] IS NULL OR
                         r.[OriginationNetwork] = '' THEN 'SIP' ELSE 'GSM' END AS Type, CASE WHEN (r.CLI LIKE '+9373%' OR
                         r.CLI LIKE '+9378%') AND (LEN(r.CLI) = 12) THEN 1 WHEN (r.CLI LIKE '+937%') AND (LEN(r.CLI) = 12) THEN 2 ELSE 3 END AS CaseAndNetworkType, CASE WHEN (r.CLI LIKE '+937%') AND (LEN(r.CLI) = 12) 
                         THEN 1 ELSE 2 END AS CaseType
FROM            dbo.RecievedCalls AS r INNER JOIN
                         dbo.GeneratedCalls AS g ON r.GeneratedCallID = g.ID
WHERE        (r.b_number LIKE '937%') AND (g.a_number <> 'Unknown') AND (r.AttemptDateTime > '2019-02-14 15:30:00') AND (r.AttemptDateTime < '2019-02-21 00:00:00') AND (r.AttemptDateTime < '2019-02-26') OR
                         (r.b_number LIKE '937%') AND (r.AttemptDateTime >= '2019-02-21 00:00:00') AND (r.AttemptDateTime < '2019-02-26')
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_Ana_AfghCDRs';


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
         Configuration = "(H (1 [50] 4 [25] 3))"
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
         Configuration = "(H (2 [66] 3))"
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
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "r"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 232
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "g"
            Begin Extent = 
               Top = 6
               Left = 270
               Bottom = 136
               Right = 525
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
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_Ana_AfghCDRs';



