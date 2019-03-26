//"use strict";

//app.directive("bpInstanceGenericbeStartbpprocessAction", ["UtilsService", "VRUIUtilsService",
//    function (UtilsService, VRUIUtilsService) {

//        var directiveDefinitionObject = {
//            restrict: "E",
//            scope: {
//                onReady: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new StartBPProcessActionCtor($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPInstance/Templates/StartBPProcessActionTemplate.html"
//        };

//        function StartBPProcessActionCtor($scope, ctrl) {
//            this.initializeController = initializeController;

//            function initializeController() {


//                $scope.scopeModel = {};

//                defineAPI();

//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var loadAPIDeferred = UtilsService.createPromiseDeferred();

//                    if (payload != undefined) {
                    
//                    }
//                    return loadAPIDeferred.promise;
//                };

//                api.getData = function () {
//                    return {
//                        $type: "Vanrise.BusinessProcess.Business.StartBPProcessAction,Vanrise.BusinessProcess.Business",
//                        //BPDefinitionId: ,
//                        //GenreicBEInputFields: 
//                    };
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }
//        }

//        return directiveDefinitionObject;
//    }]);