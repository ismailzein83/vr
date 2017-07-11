CREATE VIEW Mediation_WHS.vw_CDRs
AS
SELECT        c.CallReference AS ID, c.callreference AS IDonSwitch, c.CDPN, c.CGPN, c.MSCIncomingRoute AS In_Carrier, c.MSCOutgoingRoute AS Out_Carrier, 
                         c.MSCIncomingRoute AS In_Trunk, c.MSCOutgoingRoute AS Out_Trunk, c.ConnectDateTime AS AttemptDateTime, c.ConnectDateTime, c.DisconnectDateTime, 
                         c.DurationInSeconds, NULL AS ServiceCenter, RecordType, c.CauseForTermination AS CAUSE_FROM_RELEASE_CODE, NULL AS CAUSE_TO_RELEASE_CODE, NULL
                          AS Tag, NULL AS AlertDateTime
FROM            Mediation_WHS.CDR AS c
UNION ALL
SELECT        s.CallReference AS ID, callreference AS IDonSwitch,case when s.destinationNumber like '085%' then '26485' + substring(destinationNumber, 4, len(destinationNumber)-3) else s.destinationnumber end AS cdpn, s.ServedMSISDN AS cgpn, 'MTC_SMS' AS In_Carrier, 'TN_Mobile_SMS' AS Out_Carrier, 
                         'MTC_SMS' AS In_Trunk, 'TN_Mobile_SMS' AS Out_Trunk, s.MessageTime AS AttemptDateTime, NULL AS ConnectDateTime, NULL AS DisconnectDateTime, 
                         1 AS DurationInSeconds, s.ServiceCenter, RecordType, NULL AS CAUSE_FROM_RELEASE_CODE, NULL AS CAUSE_TO_RELEASE_CODE, NULL AS Tag, NULL 
                         AS AlertDateTime
FROM            Mobile_EDR.SMS AS s
WHERE        RecordType = 6 AND (ServiceCenter LIKE '192648119002%' OR
                         ServiceCenter LIKE '198119002%' OR
                         ServiceCenter IN ('19264851000008', '19851000008')) and (destinationnumber like '1926485%' or destinationnumber like '085%')
UNION ALL
SELECT        s.CallReference AS ID, callreference AS IDonSwitch, s.destinationnumber AS cdpn, s.ServedMSISDN AS cgpn, 'TN_Mobile_SMS' AS In_Carrier, 'MTC_SMS' AS Out_Carrier, 
                         'TN_Mobile_SMS' AS In_Trunk, 'MTC_SMS' AS Out_Trunk, s.MessageTime AS AttemptDateTime, NULL AS ConnectDateTime, NULL AS DisconnectDateTime, 
                         1 AS DurationInSeconds, s.ServiceCenter, RecordType, NULL AS CAUSE_FROM_RELEASE_CODE, NULL AS CAUSE_TO_RELEASE_CODE, NULL AS Tag, NULL 
                         AS AlertDateTime
FROM            Mobile_EDR.SMS AS s
WHERE        RecordType = 7 AND (ServiceCenter LIKE '192648119002%' OR
                         ServiceCenter LIKE '198119002%' OR
                         ServiceCenter IN ('19264851000008', '19851000008'))
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'Mediation_WHS', @level1type = N'VIEW', @level1name = N'vw_CDRs';


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
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 19
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
', @level0type = N'SCHEMA', @level0name = N'Mediation_WHS', @level1type = N'VIEW', @level1name = N'vw_CDRs';

