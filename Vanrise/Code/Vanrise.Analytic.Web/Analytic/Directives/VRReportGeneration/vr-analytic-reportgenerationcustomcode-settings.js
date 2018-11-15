"use strict";
app.directive("vrAnalyticReportgenerationcustomcodeSettings", ["UtilsService",
    function (UtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var settings = new CustomCodeSettings($scope, ctrl, $attrs);
                settings.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {
            return '<vr-tabs>'
                + '<vr-tab header = "\'Definition\'" >'
                + '   <vr-row>'
                + '        <vr-columns width="1/2row">'
                + '           <vr-textbox value="scopeModel.name" isrequired="true" label="Name"></vr-textbox>'
                + '        </vr-columns>'
                + '   </vr-row>'
                + ' </vr-tab>'
                + '<vr-tab header="\'Settings\'">'
                + '   <vr-row>'
                + '        <vr-columns width="fullrow">'
                + '             <vr-textarea value="scopeModel.customCode" isrequired label="Custom Code"></vr-textarea>'
                + '        </vr-columns>'
                + '   </vr-row>'
                + '</vr-tab>'
                + '</vr-tabs>';
        }
        function CustomCodeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {

                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    if (payload != undefined && payload.componentType != undefined) {
                        $scope.scopeModel.name = payload.componentType.Name;
                        if (payload.componentType.Settings != undefined) {
                            $scope.scopeModel.customCode = payload.componentType.Settings.CustomCode;
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };
                api.getData = function () {

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Vanrise.Analytic.Business.ReportGenerationCustomCodeSettings,Vanrise.Analytic.Business",
                            CustomCode: $scope.scopeModel.customCode
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);
