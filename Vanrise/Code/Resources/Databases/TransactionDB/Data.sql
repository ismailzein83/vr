GO
/****** Object:  Table [bp].[LKUP_ExecutionStatus]    Script Date: 12/17/2014 13:53:04 ******/
INSERT [bp].[LKUP_ExecutionStatus] ([ID], [Description]) VALUES (0, N'New')
INSERT [bp].[LKUP_ExecutionStatus] ([ID], [Description]) VALUES (10, N'Running')
INSERT [bp].[LKUP_ExecutionStatus] ([ID], [Description]) VALUES (20, N'Process Failed')
INSERT [bp].[LKUP_ExecutionStatus] ([ID], [Description]) VALUES (50, N'Completed')
INSERT [bp].[LKUP_ExecutionStatus] ([ID], [Description]) VALUES (60, N'Aborted')
INSERT [bp].[LKUP_ExecutionStatus] ([ID], [Description]) VALUES (70, N'Suspended')
INSERT [bp].[LKUP_ExecutionStatus] ([ID], [Description]) VALUES (80, N'Terminated')
/****** Object:  Table [bp].[BPDefinition]    Script Date: 12/17/2014 13:53:04 ******/
INSERT [bp].[BPDefinition] ([ID], [Title], [FQTN], [Config], [CreatedTime]) VALUES (1, N'Daily Repricing Process', N'TOne.RepricingProcess.DailyRepricingProcess, TOne.RepricingProcess', N'{"$type":"Vanrise.BusinessProcess.BPConfiguration, Vanrise.BusinessProcess","MaxConcurrentWorkflows":2,"RetryOnProcessFailed":false}', CAST(0x0000A3FC00D7BAFD AS DateTime))
INSERT [bp].[BPDefinition] ([ID], [Title], [FQTN], [Config], [CreatedTime]) VALUES (2, N'Time Range Repricing Process', N'TOne.RepricingProcess.TimeRangeRepricingProcess, TOne.RepricingProcess', N'{"$type":"Vanrise.BusinessProcess.BPConfiguration, Vanrise.BusinessProcess","MaxConcurrentWorkflows":20, "RetryOnProcessFailed":true}', CAST(0x0000A3FC00D7CE40 AS DateTime))
