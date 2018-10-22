(function (app) {
    'use strict';

    PositionedFieldsBinaryParserDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_DataParser_RecordTypeFieldType","VR_DataParser_ZeroBytesBlockAction"];

    function PositionedFieldsBinaryParserDirective(UtilsService, VRNotificationService, VRUIUtilsService, VR_DataParser_RecordTypeFieldType, VR_DataParser_ZeroBytesBlockAction) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new PositionedFieldsBinaryParser($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/PositionedFieldsBinaryRecordParser.html"
        };

        function PositionedFieldsBinaryParser($scope, ctrl, $attrs) {



            this.initializeController = initializeController;

            var recordTypeFieldTypeDirectiveAPI;
            var tagRecordParserDirectiveAPI;
            var tagCompositeFieldParserDirectiveAPI;
            var tagRecordParserDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var tagCompositeFieldParserDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeFieldSelectorAPI;
            var recordTypeFieldTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var tagRecordEntity;
            $scope.datasource = [];
            $scope.tagPositionFields = [];
            $scope.tagConstantValueFields=[];
            $scope.tagCompositeFields = [];
            function initializeController() {


                $scope.scopeModel = {};
                $scope.scopeModel.recordTypeFieldType = UtilsService.getArrayEnum(VR_DataParser_ZeroBytesBlockAction);
                $scope.scopeModel.onDataRecordTypeFieldsSelectorReady = function (api) {
                    dataRecordTypeFieldSelectorAPI = api;
                    dataRecordTypeFieldSelectorReadyPromiseDeferred.resolve();
                };

				$scope.scopeModel.onRecordTypeFieldTypeDirectiveReady = function (api) {
					recordTypeFieldTypeDirectiveAPI = api;
					recordTypeFieldTypeDirectiveReadyDeferred.resolve();
				};




                ctrl.addTempFieldName = function () {
                    $scope.datasource.push($scope.scopeModel.tempFieldName);
                    $scope.scopeModel.tempFieldName = "";
                };
                ctrl.addTagName = function () {
                            var dataItem = {
                                Position: $scope.tagPosition,
                                Length: $scope.tagLength
                            };
                   

                            dataItem.onTagRecordParserDirective = function (api) {
                        dataItem.tagRecordParserDirectiveAPI = api;
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingDirective = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.tagRecordParserDirectiveAPI, undefined, setLoader);
                    };
                    $scope.tagPositionFields.push(dataItem);
                    $scope.tagPosition = "";
                    $scope.tagLength = "";
                };




                ctrl.addConstantValue = function () {
                    var dataItem = {
                        FieldName: $scope.fieldName,
                        Value: $scope.value
                    };
                    $scope.tagConstantValueFields.push(dataItem);
                    $scope.fieldName = "";
                    $scope.value = "";
                };




                ctrl.addCompositeField = function () {
                    var dataItem = {ABC:1}; 
                    
                    dataItem.onCompositeFieldParserDirective = function (api) {
                        dataItem.tagCompositeFieldParserDirectiveAPI = api;
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingDirective = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.tagCompositeFieldParserDirectiveAPI, undefined, setLoader);
                    };
             $scope.tagCompositeFields.push(dataItem);

                };
            



                defineAPI();
            }



            function defineAPI() {
                var api = {};
                
                api.load = function (payload) {
                    var promises = [];
                    if (payload.extendedSettings != undefined) {
                        $scope.scopeModel.selectedValue = UtilsService.getItemByVal($scope.scopeModel.recordTypeFieldType, payload.extendedSettings.ZeroBytesBlockAction,"value");
                        $scope.headerLength = payload.extendedSettings.HeaderLength;
                        $scope.recordType = payload.extendedSettings.RecordType;
                        if (payload.extendedSettings.TempFieldsNames != undefined)
                            $scope.datasource = payload.extendedSettings.TempFieldsNames;
                        else
                            $scope.datasource = [];

  
                    }

                    function loadCompositeFieldGrid() {
                        if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.CompositeFieldsParsers != undefined) {


                            for (var i = 0; i < payload.extendedSettings.CompositeFieldsParsers.length; i++) {
                                var gridItem = {
                                    payload: payload.extendedSettings.CompositeFieldsParsers[i],
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                addGridItem(gridItem);


                            }

                        }

                        function addGridItem(gridItem) {
                            var dataItem = {
                                FieldName: gridItem.payload.FieldName,
                                DateTimeFieldName: gridItem.payload.DateTimeFieldName,
                                DateTimeShift: gridItem.payload.DateTimeShift
                            };
                            dataItem.onCompositeFieldParserDirective = function (api) {
                                dataItem.tagCompositeFieldParserDirectiveAPI = api;
                                gridItem.readyPromiseDeferred.resolve();
                            };

                            gridItem.readyPromiseDeferred.promise.then(function () {
                                var tagRecordParserDirectivePayload = {
                                    RecordParser: gridItem.payload
                                };
                                VRUIUtilsService.callDirectiveLoad(dataItem.tagCompositeFieldParserDirectiveAPI, tagRecordParserDirectivePayload, gridItem.loadPromiseDeferred);
                            });
                            $scope.tagCompositeFields.push(dataItem);
                        }
                    }
                    
                    function loadConstantValueGrid() {
                        if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.FieldConstantValues != undefined) {


                            for (var i = 0; i < payload.extendedSettings.FieldConstantValues.length; i++) {
                                var gridItem = {
                                    payload: payload.extendedSettings.FieldConstantValues[i],
                                    
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                
                                addGridItem(gridItem);

                            }

                        }

                        function addGridItem(gridItem) {
                            var dataItem = {
                                FieldName: gridItem.payload.FieldName,
                                Value: gridItem.payload.Value
                            };
                           $scope.tagConstantValueFields.push(dataItem);
                        }
                    }
                    function loadGrid() {
                        if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.FieldParsers != undefined) {
                           
                             
                            for (var i = 0; i < payload.extendedSettings.FieldParsers.length; i++) {
                                        var gridItem = {
                                            payload: payload.extendedSettings.FieldParsers[i],
                                            loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            readyPromiseDeferred: UtilsService.createPromiseDeferred()
                                        };
                                       addGridItem(gridItem);
                                    
                                   
                                }
                            
                        }

                        function addGridItem(gridItem) {
                            var dataItem = {
                                Position: gridItem.payload.Position,
                                Length:gridItem.payload.Length
                            };
                            dataItem.onTagRecordParserDirective = function (api) {
                                dataItem.tagRecordParserDirectiveAPI = api;
                                gridItem.readyPromiseDeferred.resolve();
                            };

                            gridItem.readyPromiseDeferred.promise.then(function () {
                                var tagRecordParserDirectivePayload = {
                                    RecordParser:gridItem.payload.FieldParser.Settings
                                };
                                VRUIUtilsService.callDirectiveLoad(dataItem.tagRecordParserDirectiveAPI, tagRecordParserDirectivePayload, gridItem.loadPromiseDeferred);
                            });
                            $scope.tagPositionFields.push(dataItem);
                        }
                    }
                 

                    loadGrid();
                    loadConstantValueGrid();
                    loadCompositeFieldGrid();

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                
                    return {
                        $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers.PositionedFieldsRecordParser, Vanrise.DataParser.MainExtensions",
                        RecordType: $scope.recordType,
                        FieldParsers: getGridData(),
                        FieldConstantValues: getGridConstantValueData(),
                        CompositeFieldsParsers: getGridCompositeData(),
                        TempFieldsNames: $scope.datasource,
                        ZeroBytesBlockAction:$scope.scopeModel.selectedValue != undefined ? $scope.scopeModel.selectedValue.value : undefined
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                function getGridData() {
                    var tagsByValue;
                    if ($scope.tagPositionFields.length > 0) {
                        tagsByValue = [];
                        for (var i = 0; i < $scope.tagPositionFields.length; i++) {
                            var tagPositionField = $scope.tagPositionFields[i];
                      
                           var tab=
                                {
                                    Position: tagPositionField.Position,
                                    Length : tagPositionField.Length,
                                    FieldParser: {Settings:tagPositionField.tagRecordParserDirectiveAPI.getData()}
                                };
                           tagsByValue.push(tab);
                        }
                    }
                    return tagsByValue;
                }
                function getGridConstantValueData() {
                    var tagsByValue;
                    if ($scope.tagConstantValueFields.length > 0) {
                        tagsByValue = [];
                        for (var i = 0; i < $scope.tagConstantValueFields.length; i++) {
                            var tagConstantValueField = $scope.tagConstantValueFields[i];
                      
                            var tab=
                                 {
                                     FieldName: tagConstantValueField.FieldName,
                                     Value: tagConstantValueField.Value,
                                   
                                 };
                            tagsByValue.push(tab);
                        }
                    }
                    return tagsByValue;
                }


                function getGridCompositeData() {
                    var tagsByValue;
                    if ($scope.tagCompositeFields.length > 0) {
                        tagsByValue = [];
                        for (var i = 0; i < $scope.tagCompositeFields.length; i++) {
                            var tagCompositeField = $scope.tagCompositeFields[i];

							var tab = tagCompositeField.tagCompositeFieldParserDirectiveAPI.getData();

							tagsByValue.push(tab);
                           
                        }
                    }
                    return tagsByValue;
                }


            }
        }
        return directiveDefinitionObject;
    }
    app.directive('vrDataparserPositionedFieldsBinaryRecordParserSettings', PositionedFieldsBinaryParserDirective);

})(app);