using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRuntime.ExecutionFlows;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.CodePreparation.BP.Arguments;
using TOne.WhS.Invoice.Business;
using TOne.WhS.Invoice.Business.Extensions;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions;
using Vanrise.GenericData.MainExtensions.DataRecordFields;
using Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEConditions;
using Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnAfterSaveHandlers;
using Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers;
using Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBESaveConditions;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.MainExtensions;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;

namespace TestRuntime.Tasks
{
    class SamerTask : ITask
    {
        public enum CaseType { Cutomer = 0, Supplier = 1,CustomerReason = 2 }

        void TestParams(string dd, params object[] values)
        {

        }
        public void Execute()
        {
           // TestParams("ff");
            #region Customer Case
         
            var customerBEDefinitionSettings = new GenericBEDefinitionSettings();
            customerBEDefinitionSettings.DataRecordStorageId = Guid.Parse("587ada97-0948-4a19-998a-9822f8c822a8");
            customerBEDefinitionSettings.DataRecordTypeId = Guid.Parse("89dcb855-4bf1-46ad-85b4-e76875acb504");
            customerBEDefinitionSettings.TitleFieldName = "OwnerReference";

            #region Customer Actions Definition
            customerBEDefinitionSettings.GenericBEActions = new List<GenericBEAction>();
            customerBEDefinitionSettings.GenericBEActions.Add(new GenericBEAction
            {
                GenericBEActionId = Guid.Parse("88758F55-2B76-455D-97DC-DA25E44D6574"),
                Name = "EditAction",
                Settings = new EditGenericBEAction()
            });
            #endregion

            customerBEDefinitionSettings.GridDefinition = new GenericBEGridDefinition();

            #region Customer Grid Columns Definition

            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions = new List<GenericBEGridColumn>();
            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "CaseId",
                FieldTitle = "ID",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "CustomerId",
                FieldTitle = "Customer",
                GridColumnSettings = new GridColumnSettings
                {
                    Width = "Normal",
                }
            });
            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "SaleZoneId",
                FieldTitle = "Sale Zone",
                GridColumnSettings = new GridColumnSettings
                {
                    Width = "Normal",
                }
            });
            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "StatusId",
                FieldTitle = "Status",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "FromDate",
                FieldTitle = "From Date",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "ToDate",
                FieldTitle = "To Date",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "Attempts",
                FieldTitle = "Attempts",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "ASR",
                FieldTitle = "ASR",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "ACD",
                FieldTitle = "ACD",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "CarrierReference",
                FieldTitle = "Carrier Reference",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "Notes",
                FieldTitle = "Notes",
                GridColumnSettings = new GridColumnSettings
                {
                    Width = "Normal",

                }
            });
            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "CreatedBy",
                FieldTitle = "Created By",
                GridColumnSettings = new GridColumnSettings
                {
                    Width = "Normal",

                }
            });
            customerBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "CreatedTime",
                FieldTitle = "Created Time",
                GridColumnSettings = new GridColumnSettings
                {
                    Width = "Normal",

                }
            });
            #endregion

            #region Customer Grid Views Definition

            customerBEDefinitionSettings.GridDefinition.GenericBEGridViews = new List<GenericBEViewDefinition>();
            customerBEDefinitionSettings.GridDefinition.GenericBEGridViews.Add(new GenericBEViewDefinition
            {
                GenericBEViewDefinitionId = Guid.NewGuid(),
                Name = "History",
                Settings = new HistoryGenericBEDefinitionView()
            });

            #endregion

            #region Customer Grid Actions Definition

            customerBEDefinitionSettings.GridDefinition.GenericBEGridActions = new List<GenericBEGridAction>();
            customerBEDefinitionSettings.GridDefinition.GenericBEGridActions.Add(new GenericBEGridAction
            {
                GenericBEGridActionId = Guid.NewGuid(),
                GenericBEActionId = Guid.Parse("88758F55-2B76-455D-97DC-DA25E44D6574"),
                ReloadGridItem = true,
                Title = "Edit"
            });

            #endregion

            #region Customer Editor Definition
            customerBEDefinitionSettings.EditorDefinition = new GenericBEEditorDefinition();
            customerBEDefinitionSettings.EditorDefinition.Settings = new StaticEditorDefinitionSetting
            {
                DirectiveName = "whs-be-casemanagement-customercase-staticeditor"
            };

            #endregion

            #region Customer Filter Definition

            customerBEDefinitionSettings.FilterDefinition = new GenericBEFilterDefinition();
            customerBEDefinitionSettings.FilterDefinition.Settings = new BasicAdvancedFilterDefinitionSettings
            {
                Filters = new List<BasicAdvancedFilterItem> {
                     new BasicAdvancedFilterItem{
                        BasicAdvancedFilterItemId = Guid.NewGuid(),
                        Name="Customer",
                        ShowInBasic = true,
                        FilterSettings = new GenericFilterDefinitionSettings{
                        FieldName= "CustomerId",
                        }
                    },
                      new BasicAdvancedFilterItem{
                        BasicAdvancedFilterItemId = Guid.NewGuid(),
                        Name="Status",
                        ShowInBasic = true,
                        FilterSettings = new GenericFilterDefinitionSettings{
                        FieldName= "StatusId",
                        }
                    },
                    new BasicAdvancedFilterItem{
                        BasicAdvancedFilterItemId = Guid.NewGuid(),
                        Name="Time",
                        ShowInBasic = true,
                        FilterSettings = new TimeFilterDefinitionSettings()
                    },
                   
                    
                     new BasicAdvancedFilterItem{
                        BasicAdvancedFilterItemId = Guid.NewGuid(),
                        Name="Group",
                        ShowInBasic = false,
                        FilterSettings = new FilterGroupFilterDefinitionSettings{
                        AvailableFieldNames =new List<string>{
                            "ReasonId",
                            "SaleZoneId",
                            "StatusId",
                            "Attempts",
                            "ASR",
                            "ACD",
                            "CarrierReference",
                            "Notes",
                            }
                        }
                    }
                }
            };
            #endregion

            customerBEDefinitionSettings.ExtendedSettings = new CustomerFaultTicketsSettings();

            #region Customer On Before Save


            customerBEDefinitionSettings.OnBeforeInsertHandler = new SerialNumberOnBeforeInsertHandler
            {
                FieldName = "OwnerReference",
                InfoType = "SerialNumberPattern",
                PartDefinitions = new List<GenericBESerialNumberPart>{
                    new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "CarrierName",
                        Settings = new GenericFieldSerialNumberPart {
                            FieldName = "CustomerId"
                        }
                    },
                    new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "CarrierProfile",
                        Settings = new GenericFieldSerialNumberPart {
                            FieldName = "CarrierProfile"
                        }
                    },
                    new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "ZoneName",
                        Settings = new GenericFieldSerialNumberPart {
                            FieldName = "SaleZoneId"
                        }
                    },new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "Counter",
                        Settings =  new SequenceSerialNumberPart{
                      InfoType = "SerialNumberInitialSequence",
                      SequenceKeyFieldName = "CustomerId",
                        }
                    },new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "Year",
                        Settings =  new DateTimeSerialNumberPart{
                              DateTimeFormat = "yyyy",
                        }
                    },new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "Month",
                        Settings =  new DateTimeSerialNumberPart{
                              DateTimeFormat = "MM",
                        }
                    },new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "Day",
                        Settings =  new DateTimeSerialNumberPart{
                              DateTimeFormat = "dd",
                        }
                    },
                   
                }
            };
            #endregion

            #region Customer On After Save

            customerBEDefinitionSettings.OnAfterSaveHandler = new ConditionalAfterSaveHandler
            {
                Handlers = new List<ConditionalAfterSaveHandlerItem>{
                    new ConditionalAfterSaveHandlerItem{
                        Name ="Status" ,
                        ConditionalAfterSaveHandlerItemId= Guid.NewGuid(),
                        Condition= new ConditionGroupSaveCondition{
                            Conditions = new List<SaveConditionItem>{
                                new SaveConditionItem{
                                    ApplicableOnOldEntity = false,
                                    Condition = new GenericFilterGroupCondition{
                                    FilterGroup = new RecordFilterGroup{
                                        FieldName = "SendEmail" ,
                                        Filters = new List<RecordFilter>{new BooleanRecordFilter{
                                        IsTrue = true ,
                                        FieldName = "SendEmail",
                                        }},
                                        LogicalOperator = RecordQueryLogicalOperator.And
                                    }
                                    } ,
                                    Name = "",
                                },
                                new SaveConditionItem{
                                    ApplicableOnOldEntity = false,
                                    Condition = new GenericFilterGroupCondition{
                                    FilterGroup = new RecordFilterGroup{
                                        FieldName = "StatusId" ,
                                        Filters = new List<RecordFilter>{new ObjectListRecordFilter{
                                        CompareOperator = ListRecordFilterOperator.In ,
                                        FieldName = "StatusId",
                                        Values = new List<object>{
                                            new Guid("7eb94106-43f1-43eb-8952-8f0b585fd7e5"),
                                        }
                                        }},
                                        LogicalOperator = RecordQueryLogicalOperator.And
                                    }
                                    } ,
                                    Name = "",
                                },
                            },
                            Operator = Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBESaveConditions.LogicalOperator.And
                        },
                        Handler = new SendEmailAfterSaveHandler{
                          EntityObjectName ="CustomerFaultTicket",
                          InfoType = "OpenTicketMailTemplate",
                        }
                    },
                     new ConditionalAfterSaveHandlerItem{
                        Name ="Status" ,
                        ConditionalAfterSaveHandlerItemId= Guid.NewGuid(),
                        Condition= new ConditionGroupSaveCondition{
                            Conditions = new List<SaveConditionItem>{
                                 new SaveConditionItem{
                                    ApplicableOnOldEntity = false,
                                    Condition = new GenericFilterGroupCondition{
                                    FilterGroup = new RecordFilterGroup{
                                        FieldName = "SendEmail" ,
                                        Filters = new List<RecordFilter>{new BooleanRecordFilter{
                                        IsTrue = true ,
                                        FieldName = "SendEmail",
                                        }},
                                        LogicalOperator = RecordQueryLogicalOperator.And
                                    }
                                    } ,
                                    Name = "",
                                },
                                new SaveConditionItem{
                                    ApplicableOnOldEntity = false,
                                    Condition = new GenericFilterGroupCondition{
                                    FilterGroup = new RecordFilterGroup{
                                        FieldName = "StatusId" ,
                                        Filters = new List<RecordFilter>{new ObjectListRecordFilter{
                                        CompareOperator = ListRecordFilterOperator.In ,
                                        FieldName = "StatusId",
                                        Values = new List<object>{
                                            new Guid("05a87955-dc2a-4e22-a879-6bea3c31690e"),
                                        }
                                        }},
                                        LogicalOperator = RecordQueryLogicalOperator.And
                                    }
                                    } ,
                                    Name = "",
                                },
                            },
                            Operator = Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBESaveConditions.LogicalOperator.And
                        },
                        Handler = new SendEmailAfterSaveHandler{
                          EntityObjectName ="CustomerFaultTicket",
                          InfoType = "PendingTicketMailTemplate",
                        }
                    },
                     new ConditionalAfterSaveHandlerItem{
                        Name ="Status" ,
                        ConditionalAfterSaveHandlerItemId= Guid.NewGuid(),
                        Condition= new ConditionGroupSaveCondition{
                            Conditions = new List<SaveConditionItem>{
                                new SaveConditionItem{
                                    ApplicableOnOldEntity = false,
                                    Condition = new GenericFilterGroupCondition{
                                    FilterGroup = new RecordFilterGroup{
                                        FieldName = "SendEmail" ,
                                        Filters = new List<RecordFilter>{new BooleanRecordFilter{
                                        IsTrue = true ,
                                        FieldName = "SendEmail",
                                        }},
                                        LogicalOperator = RecordQueryLogicalOperator.And
                                    }
                                    } ,
                                    Name = "",
                                },
                                new SaveConditionItem{
                                    ApplicableOnOldEntity = false,
                                    Condition = new GenericFilterGroupCondition{
                                    FilterGroup = new RecordFilterGroup{
                                        FieldName = "StatusId" ,
                                        Filters = new List<RecordFilter>{new ObjectListRecordFilter{
                                        CompareOperator = ListRecordFilterOperator.In ,
                                        FieldName = "StatusId",
                                        Values = new List<object>{
                                            new Guid("f299eb6d-b50c-4338-812f-142d4d8515ca"),
                                        }
                                        }},
                                        LogicalOperator = RecordQueryLogicalOperator.And
                                    }
                                    } ,
                                    Name = "",
                                },
                            },
                            Operator = Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBESaveConditions.LogicalOperator.And
                        },
                        Handler = new SendEmailAfterSaveHandler{
                          EntityObjectName ="CustomerFaultTicket",
                          InfoType = "ClosedTicketMailTemplate",
                        }
                    }
                }
            };
            #endregion

            #endregion

            var CustomerCase = Vanrise.Common.Serializer.Serialize(customerBEDefinitionSettings);


            #region Supplier Case
         
            var supplierBEDefinitionSettings = new GenericBEDefinitionSettings();
            supplierBEDefinitionSettings.DataRecordStorageId = Guid.Parse("1348be61-27b2-40dd-a50c-70f6c80c3214");
            supplierBEDefinitionSettings.DataRecordTypeId = Guid.Parse("8d7c40bc-72f6-40e4-b17e-ce5a871ba790");
            supplierBEDefinitionSettings.TitleFieldName = "OwnerReference";

            #region Supplier Actions Definition
            supplierBEDefinitionSettings.GenericBEActions = new List<GenericBEAction>();
            supplierBEDefinitionSettings.GenericBEActions.Add(new GenericBEAction
            {
                GenericBEActionId = Guid.Parse("88758F55-2B76-455D-97DC-DA25E44D6574"),
                Name = "EditAction",
                Settings = new EditGenericBEAction()
            });
            #endregion

            supplierBEDefinitionSettings.GridDefinition = new GenericBEGridDefinition();

            #region Supplier Grid Columns Definition
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions = new List<GenericBEGridColumn>();
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "CaseId",
                FieldTitle = "ID",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "SupplierId",
                FieldTitle = "Supplier",
                GridColumnSettings = new GridColumnSettings
                {
                    Width = "Normal",
                }
            });
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "SupplierZoneId",
                FieldTitle = "Supplier Zone",
                GridColumnSettings = new GridColumnSettings
                {
                    Width = "Normal",
                }
            });
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "StatusId",
                FieldTitle = "Status",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "FromDate",
                FieldTitle = "From Date",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "ToDate",
                FieldTitle = "To Date",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "Attempts",
                FieldTitle = "Attempts",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "ASR",
                FieldTitle = "ASR",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "ACD",
                FieldTitle = "ACD",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "CarrierReference",
                FieldTitle = "Carrier Reference",
                GridColumnSettings = new GridColumnSettings
                {
                    FixedWidth = 40,
                    Width = "FixedWidth"
                }
            });
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "Notes",
                FieldTitle = "Notes",
                GridColumnSettings = new GridColumnSettings
                {
                    Width = "Normal",

                }
            });
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "CreatedBy",
                FieldTitle = "Created By",
                GridColumnSettings = new GridColumnSettings
                {
                    Width = "Normal",

                }
            });
            supplierBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(new GenericBEGridColumn
            {
                FieldName = "CreatedTime",
                FieldTitle = "Created Time",
                GridColumnSettings = new GridColumnSettings
                {
                    Width = "Normal",

                }
            });

            #endregion

            #region Supplier Grid Views Definition

            supplierBEDefinitionSettings.GridDefinition.GenericBEGridViews = new List<GenericBEViewDefinition>();
            supplierBEDefinitionSettings.GridDefinition.GenericBEGridViews.Add(new GenericBEViewDefinition
            {
                GenericBEViewDefinitionId = Guid.NewGuid(),
                Name = "History",
                Settings = new HistoryGenericBEDefinitionView()
            });

            #endregion

            #region Supplier Grid Actions Definition

            supplierBEDefinitionSettings.GridDefinition.GenericBEGridActions = new List<GenericBEGridAction>();
            supplierBEDefinitionSettings.GridDefinition.GenericBEGridActions.Add(new GenericBEGridAction
            {
                GenericBEGridActionId = Guid.NewGuid(),
                GenericBEActionId = Guid.Parse("88758F55-2B76-455D-97DC-DA25E44D6574"),
                ReloadGridItem = true,
                Title = "Edit"
            });

            #endregion

            #region Supplier Editor Definition
            supplierBEDefinitionSettings.EditorDefinition = new GenericBEEditorDefinition();
            supplierBEDefinitionSettings.EditorDefinition.Settings = new StaticEditorDefinitionSetting
            {
                DirectiveName = "whs-be-casemanagement-suppliercase-staticeditor"
            };

            #endregion

            #region Supplier Filter Definition
            supplierBEDefinitionSettings.FilterDefinition = new GenericBEFilterDefinition();
            supplierBEDefinitionSettings.FilterDefinition.Settings = new BasicAdvancedFilterDefinitionSettings
            {
                Filters = new List<BasicAdvancedFilterItem> {
                     new BasicAdvancedFilterItem{
                          BasicAdvancedFilterItemId = Guid.NewGuid(),
                        Name="Supplier",
                        ShowInBasic = true,
                        FilterSettings = new GenericFilterDefinitionSettings{
                        FieldName= "SupplierId",
                        }
                    },
                    new BasicAdvancedFilterItem{
                          BasicAdvancedFilterItemId = Guid.NewGuid(),
                        Name="Status",
                        ShowInBasic = true,
                        FilterSettings = new GenericFilterDefinitionSettings{
                        FieldName= "StatusId",
                        }
                    },
                    new BasicAdvancedFilterItem{
                         BasicAdvancedFilterItemId = Guid.NewGuid(),
                        Name="Time",
                        ShowInBasic = true,
                        FilterSettings = new TimeFilterDefinitionSettings()
                    },
                     
                    
                     new BasicAdvancedFilterItem{
                          BasicAdvancedFilterItemId = Guid.NewGuid(),
                        Name="Group",
                        ShowInBasic = false,
                        FilterSettings = new FilterGroupFilterDefinitionSettings{
                        AvailableFieldNames =new List<string>{
                            "ReasonId",
                            "SupplierId",
                            "SupplierZoneId",
                            "Attempts",
                            "ASR",
                            "ACD",
                            "CarrierReference",
                            "Notes",
                            }
                        }
                    }
                }
            };



            #endregion

            supplierBEDefinitionSettings.ExtendedSettings = new SupplierFaultTicketsSettings();

            #region Supplier On Before Save


            supplierBEDefinitionSettings.OnBeforeInsertHandler = new SerialNumberOnBeforeInsertHandler
            {
                FieldName = "OwnerReference",
                InfoType = "SerialNumberPattern",
                PartDefinitions = new List<GenericBESerialNumberPart>{
                    new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "CarrierName",
                        Settings = new GenericFieldSerialNumberPart {
                            FieldName = "SupplierId"
                        }
                    },
                    new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "CarrierProfile",
                        Settings = new GenericFieldSerialNumberPart {
                            FieldName = "CarrierProfile"
                        }
                    },
                    new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "ZoneName",
                        Settings = new GenericFieldSerialNumberPart {
                            FieldName = "SupplierZoneId"
                        }
                    },new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "Counter",
                        Settings =  new SequenceSerialNumberPart{
                      InfoType = "SerialNumberInitialSequence",
                      SequenceKeyFieldName = "SupplierId",
                        }
                    },new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "Year",
                        Settings =  new DateTimeSerialNumberPart{
                              DateTimeFormat = "yyyy",
                        }
                    },new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "Month",
                        Settings =  new DateTimeSerialNumberPart{
                              DateTimeFormat = "MM",
                        }
                    },new GenericBESerialNumberPart{
                        GenericBESerialNumberPartId = Guid.NewGuid(),
                        VariableName = "Day",
                        Settings =  new DateTimeSerialNumberPart{
                              DateTimeFormat = "dd",
                        }
                    },
                }
            };
            #endregion

            #region Supplier On After Save

            supplierBEDefinitionSettings.OnAfterSaveHandler = new ConditionalAfterSaveHandler
            {
                Handlers = new List<ConditionalAfterSaveHandlerItem>{
                    new ConditionalAfterSaveHandlerItem{
                        Name ="Status" ,
                        ConditionalAfterSaveHandlerItemId= Guid.NewGuid(),
                        Condition= new ConditionGroupSaveCondition{
                            Conditions = new List<SaveConditionItem>{
                                new SaveConditionItem{
                                    ApplicableOnOldEntity = false,
                                    Condition = new GenericFilterGroupCondition{
                                    FilterGroup = new RecordFilterGroup{
                                        FieldName = "StatusId" ,
                                        Filters = new List<RecordFilter>{new ObjectListRecordFilter{
                                        CompareOperator = ListRecordFilterOperator.In ,
                                        FieldName = "StatusId",
                                        Values = new List<object>{
                                            new Guid("7eb94106-43f1-43eb-8952-8f0b585fd7e5"),
                                        }
                                        }},
                                        LogicalOperator = RecordQueryLogicalOperator.And
                                    }
                                    } ,
                                    Name = "",
                                },new SaveConditionItem{
                                    ApplicableOnOldEntity = false,
                                    Condition = new GenericFilterGroupCondition{
                                    FilterGroup = new RecordFilterGroup{
                                        FieldName = "SendEmail" ,
                                        Filters = new List<RecordFilter>{new BooleanRecordFilter{
                                        IsTrue = true ,
                                        FieldName = "SendEmail",
                                        }},
                                        LogicalOperator = RecordQueryLogicalOperator.And
                                    }
                                    } ,
                                    Name = "",
                                },
                            },
                            Operator = Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBESaveConditions.LogicalOperator.And
                        },
                        Handler = new SendEmailAfterSaveHandler{
                          EntityObjectName ="SupplierFaultTicket",
                          InfoType = "OpenTicketMailTemplate",
                        }
                    },
                     new ConditionalAfterSaveHandlerItem{
                        Name ="Status" ,
                        ConditionalAfterSaveHandlerItemId= Guid.NewGuid(),
                        Condition= new ConditionGroupSaveCondition{
                            Conditions = new List<SaveConditionItem>{
                                new SaveConditionItem{
                                    ApplicableOnOldEntity = false,
                                    Condition = new GenericFilterGroupCondition{
                                    FilterGroup = new RecordFilterGroup{
                                        FieldName = "StatusId" ,
                                        Filters = new List<RecordFilter>{new ObjectListRecordFilter{
                                        CompareOperator = ListRecordFilterOperator.In ,
                                        FieldName = "StatusId",
                                        Values = new List<object>{
                                            new Guid("05a87955-dc2a-4e22-a879-6bea3c31690e"),
                                        }
                                        }},
                                        LogicalOperator = RecordQueryLogicalOperator.And
                                    }
                                    } ,
                                    Name = "",
                                },new SaveConditionItem{
                                    ApplicableOnOldEntity = false,
                                    Condition = new GenericFilterGroupCondition{
                                    FilterGroup = new RecordFilterGroup{
                                         FieldName = "SendEmail" ,
                                        Filters = new List<RecordFilter>{new BooleanRecordFilter{
                                        IsTrue = true ,
                                        FieldName = "SendEmail",
                                        }},
                                        LogicalOperator = RecordQueryLogicalOperator.And
                                    }
                                    } ,
                                    Name = "",
                                },
                            },
                            Operator = Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBESaveConditions.LogicalOperator.And
                        },
                        Handler = new SendEmailAfterSaveHandler{
                          EntityObjectName ="SupplierFaultTicket",
                          InfoType = "PendingTicketMailTemplate",
                        }
                    },
                     new ConditionalAfterSaveHandlerItem{
                        Name ="Status" ,
                        ConditionalAfterSaveHandlerItemId= Guid.NewGuid(),
                        Condition= new ConditionGroupSaveCondition{
                            Conditions = new List<SaveConditionItem>{
                                new SaveConditionItem{
                                    ApplicableOnOldEntity = false,
                                    Condition = new GenericFilterGroupCondition{
                                    FilterGroup = new RecordFilterGroup{
                                        FieldName = "SendEmail" ,
                                        Filters = new List<RecordFilter>{new BooleanRecordFilter{
                                        IsTrue = true ,
                                        FieldName = "SendEmail",
                                        }},
                                        LogicalOperator = RecordQueryLogicalOperator.And
                                    }
                                    } ,
                                    Name = "",
                                },
                                new SaveConditionItem{
                                    ApplicableOnOldEntity = false,
                                    Condition = new GenericFilterGroupCondition{
                                    FilterGroup = new RecordFilterGroup{
                                        FieldName = "StatusId" ,
                                        Filters = new List<RecordFilter>{new ObjectListRecordFilter{
                                        CompareOperator = ListRecordFilterOperator.In ,
                                        FieldName = "StatusId",
                                        Values = new List<object>{
                                            new Guid("f299eb6d-b50c-4338-812f-142d4d8515ca"),
                                        }
                                        }},
                                        LogicalOperator = RecordQueryLogicalOperator.And
                                    }
                                    } ,
                                    Name = "",
                                },
                            },
                            Operator = Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBESaveConditions.LogicalOperator.And
                        },
                        Handler = new SendEmailAfterSaveHandler{
                          EntityObjectName ="SupplierFaultTicket",
                          InfoType = "ClosedTicketMailTemplate",
                        }
                    }
                }
            };
            #endregion

            #endregion

            var SupplierCase = Vanrise.Common.Serializer.Serialize(supplierBEDefinitionSettings);
            
            
            //var test5 = Vanrise.Common.Serializer.Serialize(eventHandler);
            //VREventHandler eventHandler = new VREventHandler
            //{
            //   Name = "InvoiceCarrierAccountStatusChanged",
            //   BED = new DateTime(2001,10,10),
            //   EED = null,
            //   VREventHandlerId = Guid.NewGuid(),
            //   Settings = new VREventHandlerSettings
            //   {
            //       ExtendedSettings = new InvoiceCarrierAccountStatusChangedHandler()
            //   }
            //};
            //var test5 = Vanrise.Common.Serializer.Serialize(eventHandler);
            //Vanrise.Common.Serializer.Serialize(eventHandler);
          //  System.Threading.ThreadPool.SetMaxThreads(10000, 10000);

            //BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            //QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            //SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };

            //var runtimeServices = new List<RuntimeService>();
            //runtimeServices.Add(queueActivationService);

            //runtimeServices.Add(bpService);

            //runtimeServices.Add(schedulerService);

            //RuntimeHost host = new RuntimeHost(runtimeServices);
            //host.Start();

            ////var tree = Vanrise.Common.Serializer.Serialize(queueFlowTree);
            //var myflow = AllFlows.GetImportCDRFlow();
            //var tree1 = Vanrise.Common.Serializer.Serialize("test");

            //AnalyticDimensionConfig AnalyticDimensionConfig = new AnalyticDimensionConfig()
            //{
            //    FieldType = new FieldTextType(),
            //    //GroupByColumns = new List<string>() { "ant.SaleZoneID", "salz.Name" },
            //    IdColumn = "ISNULL(ant.SaleZoneID,'N/A')",
            //    JoinConfigNames = new List<string>() { "SaleZoneJoin" },
            //    NameColumn = "salz.Name"

            //};
            //AnalyticDimensionConfig AnalyticDimensionConfig1 = new AnalyticDimensionConfig()
            //{
            //    FieldType = new FieldTextType(),
            //    //GroupByColumns = new List<string>() { "ant.SupplierZoneID", "suppz.Name" },
            //    IdColumn = "ISNULL(ant.SupplierZoneID,'N/A')",
            //    JoinConfigNames = new List<string>() { "SupplierZoneJoin" },
            //    NameColumn = "suppz.Name"

            //};
            //var test = Vanrise.Common.Serializer.Serialize(AnalyticDimensionConfig);
            //var test1 = Vanrise.Common.Serializer.Serialize(AnalyticDimensionConfig1);

            var runtimeServices = new List<Vanrise.Runtime.Entities.RuntimeService>();
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);
            BPRegulatorRuntimeService bpRegulatorService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationRuntimeService queueActivationRuntimeService = new QueueActivationRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };

            runtimeServices.Add(queueActivationService);
            runtimeServices.Add(schedulerService);
            runtimeServices.Add(dsRuntimeService);
            runtimeServices.Add(queueActivationRuntimeService);
            runtimeServices.Add(bpRegulatorService);
            runtimeServices.Add(queueRegulatorService);

            Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);
            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();



            //AnalyticMeasure AnalyticMeasure = new AnalyticMeasure()
            //{
            //    AnalyticMeasureConfigId = 2,
            //    Config = new AnalyticMeasureConfig()
            //    {
            //        JoinConfigNames = null,
            //        //GetSQLExpressionMethod = "",
            //        //SQLExpression = "Sum(ant.DeliveredAttempts)",
            //        //SummaryFunction = AnalyticSummaryFunction.Sum
            //    }
            //};
            //var test5 = Vanrise.Common.Serializer.Serialize(AnalyticMeasure);
            //Vanrise.Common.Serializer.Serialize(AnalyticMeasure);
               
            //QueueExecutionFlowManager executionFlowManager = new QueueExecutionFlowManager();
            //var queuesByStages = executionFlowManager.GetQueuesByStages(2);
            //CDR cdr = new CDR
            //{
            //    ID=1,
            //    Name="test"
            //};
            //while (true)
            //{
            //    //Console.ReadKey();
            //    queuesByStages["Store CDR Raws"].Queue.EnqueueObject(cdr);
            //}
            





            //BPClient bpClient = new BPClient();
            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    InputArguments = new TOne.CDRProcess.Arguments.DailyRepricingProcessInput
            //    {
            //        RepricingDay = DateTime.Parse("2014-03-01")
            //    }
            //});

            

            //BPClient bpClient = new BPClient();
            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    InputArguments = new CodePreparationProcessInput{
            //        EffectiveDate=DateTime.Now,
            //        FileId=10,
            //        SellingNumberPlanId=1
            //    }
                
            //});

        }
    }
}
