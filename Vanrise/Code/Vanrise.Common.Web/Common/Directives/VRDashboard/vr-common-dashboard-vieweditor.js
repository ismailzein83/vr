//"use strict";

//app.directive("vrCommonDashboardVieweditor", ["UtilsService", "VRUIUtilsService", "VR_GenericData_GenericBEDefinitionAPIService",
//    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {

//        var directiveDefinitionObject = {
//            restrict: "E",
//            scope:
//            {
//                onReady: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new ViewEditorCtor($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: "/Client/Modules/Common/Directives/VRDashboard/Templates/VRDashboardViewEditor.html"
//        };
//        function ViewEditorCtor($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var vrDashboardDirectiveApi;
//            var vrDashboardDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};

//                $scope.scopeModel.onVRDashboardDirectiveReady = function (api) {
//                    vrDashboardDirectiveApi = api;
//                    console.log(vrDashboardDirectiveApi);
//                    vrDashboardDirectivePromiseDeferred.resolve();
//                };

//                UtilsService.waitMultiplePromises([vrDashboardDirectivePromiseDeferred.promise]).then(function () {
//                    defineAPI();
//                });
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    console.log(payload);
//                    var promises = [];

//                    if (payload != undefined) {
//                    }
//                    promises.push(loadVRDashboardDirective());
//                   // promises.push(loadVRTileBusinessEntity());

//                    function loadVRDashboardDirective() {
//                        var vrDashboardPayload;
//                        //if (payload != undefined) {
//                        //    vrDashboardPayload = {
//                        //        businessEntityDefinitionId: '6243CA7F-A14C-41BE-BE48-86322D835CA6'
//                        //    };
//                        //}
//                        vrDashboardPayload = {
//                            businessEntityDefinitionId: '6243CA7F-A14C-41BE-BE48-86322D835CA6'
//                        };
//                        return vrDashboardDirectiveApi.load(vrDashboardPayload);
//                    }

//                    return UtilsService.waitMultiplePromises(promises);
//                };

//                api.getData = function () {
//                    console.log(vrDashboardDirectiveApi.getSelectedIds());
//                    return {
//                        $type: "Vanrise.Common.Business.VRDashboardViewSettings, Vanrise.Common.Business",
//                        DashBoardDefinitionItems: [{
//                            DashBoardDefinitionId: vrDashboardDirectiveApi.getSelectedIds(),
//                        }]
//                    };
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);

//                function loadVRTileBusinessEntity() {
//                    console.log("loadVRTileBusinessEntity");
//                    var businessEntityDefinitionId = '6243CA7F-A14C-41BE-BE48-86322D835CA6';
//                    return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEGridDefinition(businessEntityDefinitionId).then(function (response) {
//                        console.log(response);
//                    });
//                }
//            };


//        }

//        return directiveDefinitionObject;
//    }
//]);