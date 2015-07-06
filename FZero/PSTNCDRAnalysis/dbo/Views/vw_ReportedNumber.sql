
CREATE VIEW [dbo].[vw_ReportedNumber]
AS
SELECT    top 10000000 dbo.NumberProfile.SubscriberNumber as CLI, dbo.NumberProfile.Date_Day, (CASE WHEN Total_Out_Volume = - 1 THEN 0 ELSE Total_Out_Volume END) AS OriginatedVolume, 
                      (CASE WHEN Total_In_Volume = - 1 THEN 0 ELSE Total_In_Volume END) AS TerminatedVolume, (CASE WHEN Count_Out = - 1 THEN 0 ELSE Count_Out END) 
                      AS NumberofAttempts, (CASE WHEN Diff_Output_Numb_ = - 1 THEN 0 ELSE Diff_Output_Numb_ END) AS CountDistinctCalledParties, dbo.Report.Id AS ReportID, 
                      CONVERT(char(4), dbo.NumberProfile.Date_Day, 107) + '' + CAST(DATEPART(year, dbo.NumberProfile.Date_Day) AS varchar(4)) AS MonthYear
FROM         dbo.Report INNER JOIN
                      dbo.ReportDetails ON dbo.Report.Id = dbo.ReportDetails.ReportId INNER JOIN
                      dbo.NumberProfile ON dbo.ReportDetails.SubscriberNumber = dbo.NumberProfile.SubscriberNumber
WHERE     (dbo.NumberProfile.Day_Hour = 30)
 order by  dbo.NumberProfile.Date_Day asc
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_ReportedNumber';


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
         Begin Table = "Report"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 198
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ReportDetails"
            Begin Extent = 
               Top = 6
               Left = 236
               Bottom = 125
               Right = 412
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "NumberProfile"
            Begin Extent = 
               Top = 6
               Left = 450
               Bottom = 125
               Right = 636
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_ReportedNumber';

