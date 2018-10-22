"use strict";

app.directive("vrDataparserCreateRecordBinaryRecordParserSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new createRecordParserEditor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/CreateRecordBinaryRecordParser.html"

    };

    function createRecordParserEditor($scope, ctrl) {

        var context;
       
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.fieldsParsers = [];
            $scope.scopeModel.tagConstantValueFields = [];
            $scope.scopeModel.compositeFieldsParsers = [];
            $scope.datasource = [];
          
          ctrl.addFieldParser = function () {
                var dataItem = { FieldParser: $scope.scopeModel.filedParser };
                dataItem.onFieldRecordParserDirective = function (api) {
                   
                    dataItem.fieldRecordParserDirectiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.fieldRecordParserDirectiveAPI, undefined, setLoader);
                };
                $scope.scopeModel.fieldsParsers.push(dataItem);
                $scope.scopeModel.filedParser = "";
          };
          ctrl.addFieldConstantValue = function () {
              var dataItem = {
                  FieldName: $scope.scopeModel.fieldName,
                  Value: $scope.scopeModel.value
              };
              $scope.scopeModel.tagConstantValueFields.push(dataItem);
              $scope.scopeModel.fieldName = "";
              $scope.scopeModel.value = "";
          };
			ctrl.addCompositeFieldParser = function () {
				var dataItem = {};
				dataItem.onCompositeFieldsParsersDirective = function (api) {
					dataItem.compositeFieldsParsersDirectiveAPI = api;
					var setLoader = function (value) {
						$scope.scopeModel.isLoadingDirective = value;
					};
					VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.compositeFieldsParsersDirectiveAPI, undefined, setLoader);
				};
				$scope.scopeModel.compositeFieldsParsers.push(dataItem);

			};
          ctrl.addTempFieldName = function () {
              $scope.datasource.push($scope.scopeModel.tempFieldName);
              $scope.scopeModel.tempFieldName = "";
          };


            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    if (payload.extendedSettings != undefined)
                    { $scope.scopeModel.recordType = payload.extendedSettings.RecordType; }
                    if (payload.extendedSettings!=undefined && payload.extendedSettings.TempFieldsNames != undefined)
                        $scope.datasource = payload.extendedSettings.TempFieldsNames;
                    else
                        $scope.datasource = [];
                    context = payload.context;

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
                        $scope.scopeModel.tagConstantValueFields.push(dataItem);
                    }
                }

                function loadGrid() {
                    
                    if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.FieldParsers != undefined && payload.extendedSettings.FieldParsers.FieldParsersByTag != undefined) {
                        for (var tag in payload.extendedSettings.FieldParsers.FieldParsersByTag) {
                            if (tag != "$type") {
                                var gridItem = {
                                    payload: {
                                        key: tag,
                                        value: payload.extendedSettings.FieldParsers.FieldParsersByTag[tag]
                                    },
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                //   promises.push(gridItem.loadPromiseDeferred.promise);
                                addGridItem(gridItem);
                            }
                        }
                    }

                    function addGridItem(gridItem) {
                        var dataItem = {
                            FieldParser: gridItem.payload.key
                        };

                        dataItem.onFieldRecordParserDirective = function (api) {
                            dataItem.fieldRecordParserDirectiveAPI = api;
                            gridItem.readyPromiseDeferred.resolve();
                        };

                        gridItem.readyPromiseDeferred.promise.then(function () {
                            var tagRecordParserDirectivePayload = {
                                RecordParser: gridItem.payload.value.Settings
                            };
                            VRUIUtilsService.callDirectiveLoad(dataItem.fieldRecordParserDirectiveAPI, tagRecordParserDirectivePayload, gridItem.loadPromiseDeferred);
                        });
                        $scope.scopeModel.fieldsParsers.push(dataItem);
                    }
                }








                function loadCompositeFieldsGrid() {
                    
                    if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.CompositeFieldsParsers != undefined) {
                        for(var i=0;i<payload.extendedSettings.CompositeFieldsParsers.length;i++)
                        {
							var gridItem = {
								payload: { key: i, value: payload.extendedSettings.CompositeFieldsParsers[i] },
								loadPromiseDeferred: UtilsService.createPromiseDeferred(),
								readyPromiseDeferred: UtilsService.createPromiseDeferred()

							};
                         
							addGridItem(gridItem);  
                        }

                    }
                

                function addGridItem(gridItem) {
                    var dataItem = {};

                    dataItem.onCompositeFieldsParsersDirective = function (api) {
                        dataItem.compositeFieldsParsersDirectiveAPI = api;
                        gridItem.readyPromiseDeferred.resolve();
                    };

                    gridItem.readyPromiseDeferred.promise.then(function () {
                        var compositeFieldRecordParserDirectivePayload = {
                            RecordParser: gridItem.payload.value
                        };
                        VRUIUtilsService.callDirectiveLoad(dataItem.compositeFieldsParsersDirectiveAPI, compositeFieldRecordParserDirectivePayload, gridItem.loadPromiseDeferred);
                    });
                    $scope.scopeModel.compositeFieldsParsers.push(dataItem);
                }
            }
        







           loadGrid();
           loadConstantValueGrid();
           loadCompositeFieldsGrid();
                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
                
				return {
					$type: "Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.RecordParsers.CreateRecordRecordParser,Vanrise.DataParser.MainExtensions",
					RecordType: $scope.scopeModel.recordType,
					FieldParsers: { FieldParsersByTag: GetFieldsParserGrid() },
					FieldConstantValues: getGridConstantValueData(),
					CompositeFieldsParsers: GetCompositeFieldsParserGrid(),
					TempFieldsNames: $scope.datasource
				};
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
            function GetFieldsParserGrid() {
                var tagsByValue;
                if ($scope.scopeModel.fieldsParsers.length > 0) {
                    tagsByValue = {};
                    for (var i = 0; i < $scope.scopeModel.fieldsParsers.length; i++) {
                        var tagName = $scope.scopeModel.fieldsParsers[i];
                        tagsByValue[tagName.FieldParser] = { Settings: tagName.fieldRecordParserDirectiveAPI.getData() };
                    }
                }
                return tagsByValue;

            }

            function GetCompositeFieldsParserGrid()
            {
                var compositeFields;
                if($scope.scopeModel.compositeFieldsParsers.length>0)
                {
                    compositeFields =[];
                    for(var i=0;i<$scope.scopeModel.compositeFieldsParsers.length;i++)
                    {
                        var compositeField = $scope.scopeModel.compositeFieldsParsers[i];
                        compositeFields.push(compositeField.compositeFieldsParsersDirectiveAPI.getData());
                    }

                }

                return compositeFields;

            }


            function getGridConstantValueData() {
                var tagsByValue;
                if ($scope.scopeModel.tagConstantValueFields.length > 0) {
                    tagsByValue = [];
                    for (var i = 0; i < $scope.scopeModel.tagConstantValueFields.length; i++) {
                        var tagConstantValueField = $scope.scopeModel.tagConstantValueFields[i];

                        var tab =
                             {
                                 FieldName: tagConstantValueField.FieldName,
                                 Value: tagConstantValueField.Value,

                             };
                        tagsByValue.push(tab);
                    }
                }
                return tagsByValue;
            }
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }




    return directiveDefinitionObject;

}]);