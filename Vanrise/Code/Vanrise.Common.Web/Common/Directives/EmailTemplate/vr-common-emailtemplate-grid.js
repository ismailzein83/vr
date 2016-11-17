"use strict";

app.directive("vrCommonEmailtemplateGrid", ["UtilsService", "VRNotificationService", "VRCommon_EmailTemplateAPIService", "VRCommon_EmailTemplateService",
function (UtilsService, VRNotificationService, VRCommon_EmailTemplateAPIService, VRCommon_EmailTemplateService) {
    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new EmailTemplateGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/EmailTemplate/Templates/EmailTemplateGridTemplate.html"

    };

    function EmailTemplateGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.emailTemplates = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_EmailTemplateAPIService.GetFilteredEmailTemplates(dataRetrievalInput)
                    .then(function (response) {

                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }



        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editEmailTemplate,
                haspermission: hasUpdateEmailTemplatePermission
            }];
        }

        function hasUpdateEmailTemplatePermission() {
            return VRCommon_EmailTemplateAPIService.HasUpdateEmailTemplatePermission();
        }

        function editEmailTemplate(emailTemplate) {
            var onEmailTemplateUpdated = function (emailTemplateObj) {
                gridAPI.itemUpdated(emailTemplateObj);
            };
            VRCommon_EmailTemplateService.editEmailTemplate(emailTemplate.Entity.EmailTemplateId, onEmailTemplateUpdated);
        }
    }

    return directiveDefinitionObject;

}]);
