'use strict';
app.directive('vrWhsBeCarrierprofile', ['WhS_BE_CarrierProfileAPIService', 'UtilsService', '$compile',
function (WhS_BE_CarrierProfileAPIService, UtilsService, $compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onloaded: '=',
            ismultipleselection: "@",
            isdisabled: "=",
            onselectionchanged: '=',
            isrequired: "@",

        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            $scope.selectedCarrierProfiles;
            if ($attrs.ismultipleselection != undefined)
                $scope.selectedCarrierProfiles = [];

            $scope.carrierProfiles = [];
            var beCarrierProfileObject = new beCarrierProfile(ctrl, $scope, $attrs);
            beCarrierProfileObject.initializeController();
            $scope.onselectionchanged = function () {

                if (ctrl.onselectionchanged != undefined) {
                    var onvaluechangedMethod = $scope.$parent.$eval(ctrl.onselectionchanged);
                    if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                        onvaluechangedMethod();
                    }
                }

            }
        },
        controllerAs: 'ctrl',
        bindToController: true,
        link: function preLink($scope, iElement, iAttrs) {
            var ctrl = $scope.ctrl;
            $scope.$watch('ctrl.isdisabled', function () {
                var template = getBeCarrierProfileTemplate(iAttrs, ctrl);
                iElement.html(template);
                $compile(iElement.contents())($scope);
            });
        }

    };


    function getBeCarrierProfileTemplate(attrs, ctrl) {

        var multipleselection = "";
        if (attrs.ismultipleselection != undefined)
            multipleselection = "ismultipleselection"
        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";
        var disabled = "";
        if (ctrl.isdisabled)
            disabled = "vr-disabled='true'"
        return '<div  vr-loader="isLoadingDirective">'
            + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="CarrierProfileId" '
        + required + ' label="Carrier Profile" datasource="carrierProfiles" selectedvalues="selectedCarrierProfiles"  onselectionchanged="onselectionchanged" ' + disabled + '></vr-select>'
           + '</div>'
    }

    function beCarrierProfile(ctrl, $scope, $attrs) {

        function initializeController() {

            loadCarrierProfiles();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return $scope.selectedCarrierProfiles;
            }

            api.setData = function (selectedIds) {

                if ($attrs.ismultipleselection) {
                    for (var i = 0; i < selectedIds.length; i++) {
                        var selectedCarrierProfile = UtilsService.getItemByVal($scope.carrierProfiles, selectedIds[i], "CarrierProfileId");
                        if (selectedCarrierProfile != null)
                            $scope.selectedCarrierProfiles.push(selectedCarrierProfile);
                    }
                }
                else {
                    var selectedCarrierProfile = UtilsService.getItemByVal($scope.carrierProfiles, selectedIds, "CarrierProfileId");
                    if (selectedCarrierProfile != null)
                        $scope.selectedCarrierProfiles = selectedCarrierProfile;
                }
            }

            if (ctrl.onloaded != null)
                ctrl.onloaded(api);
        }
        function loadCarrierProfiles() {
            $scope.isLoadingDirective = true;
            return WhS_BE_CarrierProfileAPIService.GetAllCarrierProfiles().then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.carrierProfiles.push(itm);
                });
            }).catch(function (error) {
                //TODO handle the case of exceptions

            }).finally(function () {
                $scope.isLoadingDirective = false;
                defineAPI();
            });
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);