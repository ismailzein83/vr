
CREATE VIEW [dbo].[ViewGeneratedCalls]
AS
SELECT     dbo.RecievedCalls.ClientID, dbo.GeneratedCalls.SourceID, 'Case' + CONVERT(char(20), dbo.GeneratedCalls.ID) AS CaseID, dbo.GeneratedCalls.MobileOperatorID, 
                      dbo.GeneratedCalls.StatusID, dbo.GeneratedCalls.PriorityID, dbo.GeneratedCalls.ReportingStatusID, dbo.RecievedCalls.DurationInSeconds, 
                      dbo.GeneratedCalls.MobileOperatorFeedbackID, dbo.GeneratedCalls.OriginationNetwork, dbo.GeneratedCalls.AssignedTo, dbo.GeneratedCalls.AssignedBy, 
                      dbo.GeneratedCalls.ReportID, dbo.GeneratedCalls.AttemptDateTime, dbo.GeneratedCalls.FeedbackDateTime, dbo.GeneratedCalls.AssignmentDateTime, 
                      dbo.ReportingStatuses.Name AS ReportingStatusName, dbo.Priorities.Name AS PriorityName, dbo.Reports.ReportID AS ReportRealID, 
                      Sources_1.Name AS SourceName, dbo.Statuses.Name AS StatusName, Users_1.FullName AS AssignedToFullName, dbo.GeneratedCalls.a_number, 
                      dbo.GeneratedCalls.b_number, dbo.Users.FullName AS MobileOperatorName, dbo.RecommendedActions.Name AS RecommendedActionName, 
                      dbo.Reports.SentDateTime AS ReportingDateTime, Users_2.FullName AS ReportedByName, Users_2.MobileNumber, dbo.Reports.RecommendedActionID, 
                      dbo.GeneratedCalls.Level1Comparison, dbo.GeneratedCalls.Level2Comparison, dbo.Sources.Name AS ReceivedSourceName, 
                      dbo.RecievedCalls.SourceID AS ReceivedSourceID, dbo.RecievedCalls.CLI, dbo.GeneratedCalls.ToneFeedbackID, dbo.GeneratedCalls.LevelOneComparisonDateTime, 
                      dbo.GeneratedCalls.LevelTwoComparisonDateTime, dbo.ToneFeedbacks.Name AS ToneFeedbackName, 
                      dbo.RecievedCalls.MobileOperatorID AS ReceivedMobileOperatorID, Users_3.FullName AS ReceivedMobileOperatorFeedbackName, dbo.GeneratedCalls.ID, 
                      dbo.fn_CheckIfReportedBefore(dbo.RecievedCalls.CLI) AS ReportedBefore, dbo.fn_CheckIfToBeReportedBefore(dbo.RecievedCalls.CLI) AS ToBeReportedBefore, 
                      dbo.GeneratedCalls.FeedbackNotes, dbo.MobileOperatorFeedbacks.Name AS MobileOperatorFeedbackName, dbo.GeneratedCalls.Carrier
FROM         dbo.ReportingStatuses RIGHT OUTER JOIN
                      dbo.Statuses RIGHT OUTER JOIN
                      dbo.Reports LEFT OUTER JOIN
                      dbo.ApplicationUsers ON dbo.Reports.ApplicationUserID = dbo.ApplicationUsers.ID RIGHT OUTER JOIN
                      dbo.Users AS Users_1 RIGHT OUTER JOIN
                      dbo.ApplicationUsers AS ApplicationUsers_1 ON Users_1.ID = ApplicationUsers_1.UserID RIGHT OUTER JOIN
                      dbo.GeneratedCalls LEFT OUTER JOIN
                      dbo.MobileOperatorFeedbacks ON dbo.GeneratedCalls.MobileOperatorFeedbackID = dbo.MobileOperatorFeedbacks.ID ON 
                      ApplicationUsers_1.ID = dbo.GeneratedCalls.AssignedTo LEFT OUTER JOIN
                      dbo.MobileOperators LEFT OUTER JOIN
                      dbo.Users ON dbo.MobileOperators.UserID = dbo.Users.ID ON dbo.GeneratedCalls.MobileOperatorID = dbo.MobileOperators.ID LEFT OUTER JOIN
                      dbo.ToneFeedbacks ON dbo.GeneratedCalls.ToneFeedbackID = dbo.ToneFeedbacks.ID LEFT OUTER JOIN
                      dbo.Users AS Users_3 RIGHT OUTER JOIN
                      dbo.MobileOperators AS MobileOperators_1 RIGHT OUTER JOIN
                      dbo.RecievedCalls ON MobileOperators_1.ID = dbo.RecievedCalls.MobileOperatorID ON Users_3.ID = MobileOperators_1.UserID ON 
                      dbo.GeneratedCalls.ID = dbo.RecievedCalls.GeneratedCallID ON dbo.Reports.ID = dbo.GeneratedCalls.ReportID LEFT OUTER JOIN
                      dbo.RecommendedActions ON dbo.Reports.RecommendedActionID = dbo.RecommendedActions.ID ON 
                      dbo.Statuses.ID = dbo.GeneratedCalls.StatusID LEFT OUTER JOIN
                      dbo.Sources AS Sources_1 ON dbo.GeneratedCalls.SourceID = Sources_1.ID ON dbo.ReportingStatuses.ID = dbo.GeneratedCalls.ReportingStatusID LEFT OUTER JOIN
                      dbo.Priorities ON dbo.GeneratedCalls.PriorityID = dbo.Priorities.ID LEFT OUTER JOIN
                      dbo.Sources ON dbo.RecievedCalls.SourceID = dbo.Sources.ID LEFT OUTER JOIN
                      dbo.Users AS Users_2 ON dbo.ApplicationUsers.UserID = Users_2.ID
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 3, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'ViewGeneratedCalls';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane3', @value = N'
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
         Width = 1875
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
         Column = 5895
         Alias = 4215
         Table = 2610
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'ViewGeneratedCalls';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'        End
            DisplayFlags = 280
            TopColumn = 11
         End
         Begin Table = "MobileOperatorFeedbacks"
            Begin Extent = 
               Top = 173
               Left = 407
               Bottom = 268
               Right = 702
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MobileOperators"
            Begin Extent = 
               Top = 189
               Left = 588
               Bottom = 308
               Right = 915
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Users"
            Begin Extent = 
               Top = 122
               Left = 969
               Bottom = 241
               Right = 1136
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ToneFeedbacks"
            Begin Extent = 
               Top = 157
               Left = 735
               Bottom = 246
               Right = 895
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Users_3"
            Begin Extent = 
               Top = 194
               Left = 1068
               Bottom = 313
               Right = 1235
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MobileOperators_1"
            Begin Extent = 
               Top = 272
               Left = 966
               Bottom = 361
               Right = 1237
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "RecievedCalls"
            Begin Extent = 
               Top = 74
               Left = 825
               Bottom = 297
               Right = 1014
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "RecommendedActions"
            Begin Extent = 
               Top = 29
               Left = 1065
               Bottom = 118
               Right = 1225
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Sources_1"
            Begin Extent = 
               Top = 69
               Left = 865
               Bottom = 161
               Right = 1025
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Priorities"
            Begin Extent = 
               Top = 51
               Left = 791
               Bottom = 140
               Right = 951
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Sources"
            Begin Extent = 
               Top = 49
               Left = 799
               Bottom = 168
               Right = 966
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Users_2"
            Begin Extent = 
               Top = 52
               Left = 790
               Bottom = 267
               Right = 957
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
      Begin ColumnWidths = 43
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
         Width = 1500', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'ViewGeneratedCalls';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[17] 4[32] 2[30] 3) )"
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
         Begin Table = "ReportingStatuses"
            Begin Extent = 
               Top = 114
               Left = 868
               Bottom = 203
               Right = 1028
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Statuses"
            Begin Extent = 
               Top = 31
               Left = 924
               Bottom = 120
               Right = 1084
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Reports"
            Begin Extent = 
               Top = 59
               Left = 775
               Bottom = 180
               Right = 1021
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "ApplicationUsers"
            Begin Extent = 
               Top = 116
               Left = 795
               Bottom = 205
               Right = 955
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Users_1"
            Begin Extent = 
               Top = 181
               Left = 872
               Bottom = 300
               Right = 1039
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ApplicationUsers_1"
            Begin Extent = 
               Top = 85
               Left = 795
               Bottom = 289
               Right = 968
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "GeneratedCalls"
            Begin Extent = 
               Top = 12
               Left = 41
               Bottom = 309
               Right = 282
    ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'ViewGeneratedCalls';

