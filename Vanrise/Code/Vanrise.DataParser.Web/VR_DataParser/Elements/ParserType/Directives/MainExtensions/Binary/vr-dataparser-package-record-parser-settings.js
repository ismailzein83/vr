(function (app) {
    'use strict';

   PackageRecordParserDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_DataParser_RecordTypeFieldType"];

   function PackageRecordParserDirective(UtilsService, VRNotificationService, VRUIUtilsService, VR_DataParser_RecordTypeFieldType) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new PackageRecordParser($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/PackageRecordParser.html"

        };

        function PackageRecordParser($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            var tagRecordParserDirectiveAPI;
            var tagRecordParserDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var tagRecordEntity;
            function initializeController() {


                $scope.scopeModel = {};
              
                $scope.scopeModel.packages = [];

                ctrl.addPackage = function () {
                    var dataItem = { Package: $scope.scopeModel.package };
                    dataItem.onPackageRecordParserDirective = function (api) {
                        dataItem.packageRecordParserDirectiveAPI = api;
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingDirective = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.packageRecordParserDirectiveAPI, undefined, setLoader);
                    };
                    $scope.scopeModel.packages.push(dataItem);
                    $scope.scopeModel.package = "";
                };







                $scope.scopeModel.compositeFieldsParsers = [];

                ctrl.addCompositeFieldParser = function () {
                    var dataItem = {};
                    dataItem.onCompositeFieldsRecordParserDirective = function (api) {
                        dataItem.compositeFieldRecordParserDirectiveAPI = api;
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingDirective = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.compositeFieldRecordParserDirectiveAPI, undefined, setLoader);
                    };
                    $scope.scopeModel.compositeFieldsParsers.push(dataItem);
                    $scope.scopeModel.compositeFieldsParsersCompositeFieldsParsers = "";
                };




                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload.extendedSettings != undefined) {
                        $scope.scopeModel.recordType = payload.extendedSettings.RecordType;
                        $scope.scopeModel.packageTagLength = payload.extendedSettings.PackageTagLength;
                        $scope.scopeModel.packageLengthByteLength = payload.extendedSettings.PackageLengthByteLength;

                    }




                    function loadPackagesGrid() {
                        if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.Packages != undefined) {
                            for (var tag in payload.extendedSettings.Packages) {
                                if (tag != "$type") {
                                    var gridItem = {
                                        payload: {
                                            key: tag,
                                            value: payload.extendedSettings.Packages[tag]
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
                                Package: gridItem.payload.key
                            };

                            dataItem.onPackageRecordParserDirective = function (api) {
                                dataItem.packageRecordParserDirectiveAPI = api;
                                gridItem.readyPromiseDeferred.resolve();
                            };

                            gridItem.readyPromiseDeferred.promise.then(function () {
                                var packageRecordParserDirectivePayload = {
                                    RecordParser: gridItem.payload.value
                                };
                                VRUIUtilsService.callDirectiveLoad(dataItem.packageRecordParserDirectiveAPI, packageRecordParserDirectivePayload, gridItem.loadPromiseDeferred);
                            });
                            $scope.scopeModel.packages.push(dataItem);
                        }
                    }

                    function loadCompositeFieldsGrid() {
                        if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.CompositeFieldsParsers != undefined) {
                            for (var tag in payload.extendedSettings.CompositeFieldsParsers) {
                                if (tag != "$type") {
                                    var gridItem = {
                                        payload: {
                                          
                                            value: payload.extendedSettings.CompositeFieldsParsers[tag]
                                        },
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    addGridItem(gridItem);
                                }
                            }
                        }

                        function addGridItem(gridItem) {
                            var dataItem = {};
                           
                            dataItem.onCompositeFieldsRecordParserDirective = function (api) {
                                dataItem.compositeFieldRecordParserDirectiveAPI = api;
                                gridItem.readyPromiseDeferred.resolve();
                            };

                            gridItem.readyPromiseDeferred.promise.then(function () {
                                var packageRecordParserDirectivePayload = {
                                    RecordParser: gridItem.payload.value
                                };
                                VRUIUtilsService.callDirectiveLoad(dataItem.compositeFieldRecordParserDirectiveAPI, packageRecordParserDirectivePayload, gridItem.loadPromiseDeferred);
                            });
                            $scope.scopeModel.compositeFieldsParsers.push(dataItem);
                        }
                    }
                    loadPackagesGrid();
                    loadCompositeFieldsGrid();
                    


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.RecordParsers.PackageRecordParser,Vanrise.DataParser.MainExtensions",
                        RecordType:$scope.scopeModel.recordType,
                        Packages: getPackagesGridData(),
                        PackageTagLength: $scope.scopeModel.packageTagLength,
                        PackageLengthByteLength: $scope.scopeModel.packageLengthByteLength,
                        CompositeFieldsParsers: getCompositeFieldsGridData()

                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                function getPackagesGridData() {
                    var tagsByValue;
                    if ($scope.scopeModel.packages.length > 0) {
                        tagsByValue = {};
                        for (var i = 0; i < $scope.scopeModel.packages.length; i++) {
                            var pack = $scope.scopeModel.packages[i];
                            tagsByValue[pack.Package] = pack.packageRecordParserDirectiveAPI.getData();

                        }
                    }
                    return tagsByValue;
                }
                function getCompositeFieldsGridData() {
                    var tagsByValue;
                    if ($scope.scopeModel.compositeFieldsParsers.length > 0) {
                        tagsByValue = [];
                        for (var i = 0; i < $scope.scopeModel.compositeFieldsParsers.length; i++) {
                            var pack = $scope.scopeModel.compositeFieldsParsers[i];
                            tagsByValue.push(pack.compositeFieldRecordParserDirectiveAPI.getData());

                        }
                    }
                    return tagsByValue;
                }
            }
        }
        return directiveDefinitionObject;
    }
   app.directive('vrDataparserPackageRecordParserSettings', PackageRecordParserDirective);

})(app);