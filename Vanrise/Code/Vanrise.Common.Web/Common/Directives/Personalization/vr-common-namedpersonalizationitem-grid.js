"use strict";

app.directive("vrCommonNamedpersonalizationitemGrid", ["UtilsService", "VRNotificationService", "VR_Common_EntityPersonalizationAPIService", "VRCommon_CountryService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, VR_Common_EntityPersonalizationAPIService, VRCommon_CountryService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            onsettingsclicked: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new PersonalizationItemGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/Personalization/Templates/NamedPersonalizationItemGridTemplate.html"

    };

    function PersonalizationItemGrid($scope, ctrl, $attrs) {

        ctrl.onSettingClick = function (dataItem) {
            if (ctrl.onsettingsclicked != undefined && typeof (ctrl.onsettingsclicked) == "function") {
                var data = {
                    Name: dataItem.Name,
                    NamedEntityPersonalizations: dataItem.NamedEntityPersonalizations,
                    IsGlobal: dataItem.IsGlobal
                };
                return ctrl.onsettingsclicked(data);
            }               
        };

        ctrl.markSelectedSettingRow = function (dataItem) {
            if (dataItem.Name == selectedName && selectedIsGlobal == dataItem.IsGlobal) {
                return { CssClass: 'alert-success' };
            }
        };
        var gridAPI;
        this.initializeController = initializeController;
        var selectedName;
        var selectedIsGlobal;
        function initializeController() {
           
            $scope.personalizationItems = [];
            $scope.onGridReady = function (api) {

                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                   
                    var directiveAPI = {};

                    directiveAPI.load = function (payload) {
                        var query;
                        if (payload != undefined) {
                            query = payload.query;
                            selectedName = payload.selectedName;
                            selectedIsGlobal = payload.selectedIsGlobal;
                        }
                        return gridAPI.retrieveData(query);

                    };

                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Common_EntityPersonalizationAPIService.GetFilteredNamedEntityPersonalization(dataRetrievalInput)
                    .then(function (response) {                          
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };            
        }
              
    }

    return directiveDefinitionObject;

}]);
