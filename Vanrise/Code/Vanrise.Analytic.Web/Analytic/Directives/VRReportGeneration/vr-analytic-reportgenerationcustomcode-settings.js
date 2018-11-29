"use strict";
app.directive("vrAnalyticReportgenerationcustomcodeSettings", ["UtilsService", "VR_Analytic_ReportGenerationCustomCodeAPIService","VR_Analytic_ReportGenerationCustomCodeService",
    function (UtilsService, VR_Analytic_ReportGenerationCustomCodeAPIService, VR_Analytic_ReportGenerationCustomCodeService) {
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
                + '        <vr-columns width="1/2row">'
                + '             <vr-textarea value="scopeModel.customCode" isrequired label="Custom Code" rows="20"></vr-textarea>'
                + '        </vr-columns>'
                + '        <vr-columns width="1/2row">'
                + '             <vr-textarea value="scopeModel.classes" label="Classes" rows="20"></vr-textarea>'
                + '        </vr-columns>'
                + '   </vr-row>'
                + '</vr-tab>'
                + '</vr-tabs>';
        }
        function CustomCodeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;

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
                            $scope.scopeModel.classes = payload.componentType.Settings.Classes;
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };
                api.validate = function () {
                    var validatePromise = UtilsService.createPromiseDeferred();
                    var input = {
                        CustomCode: $scope.scopeModel.customCode,
                        Classes: $scope.scopeModel.classes
                    };
                    VR_Analytic_ReportGenerationCustomCodeAPIService.TryCompileCustomCode(input).then(function (output) {
                        if (output != undefined) {
                            if (output.CompilationSucceeded) {
                                validatePromise.resolve(true);
                            }
                            else {
                                VR_Analytic_ReportGenerationCustomCodeService.showCustomCodeCompilationErrors(output.ErrorMessages, getContext());
                                validatePromise.reject();
                            }
                        }
                        else {
                            validatePromise.reject();
                        }
                    });
                    return validatePromise.promise;
                };
                api.getData = function () {
                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Vanrise.Analytic.Business.ReportGenerationCustomCodeSettings,Vanrise.Analytic.Business",
                            CustomCode: $scope.scopeModel.customCode,
                            Classes: $scope.scopeModel.classes
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;
    }
]);
