(function (appControllers) {

    'use strict';

    httpProxyMethodEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRCommon_VRHttpMethodEnum', 'VRCommon_VRHttpMessageFormatEnum'];

    function httpProxyMethodEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRCommon_VRHttpMethodEnum, VRCommon_VRHttpMessageFormatEnum) {

        var context;
        var httpProxyMethodEntity;
        var isEditMode;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                httpProxyMethodEntity = parameters.httpProxyMethodEntity;
            }

            isEditMode = httpProxyMethodEntity != undefined;
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.headers = [];
            $scope.scopeModel.parameters = [];
            $scope.scopeModel.urlParameters = [];
            $scope.scopeModel.isBodyDisabled = false;

            $scope.scopeModel.httpMethodTypes = UtilsService.getArrayEnum(VRCommon_VRHttpMethodEnum);
            $scope.scopeModel.httpMessageFormats = UtilsService.getArrayEnum(VRCommon_VRHttpMessageFormatEnum);

            $scope.scopeModel.selectedHttpMethodFormat = VRCommon_VRHttpMessageFormatEnum.ApplicationJSON;
            $scope.scopeModel.addHeader = function () {
                var header = {};
                $scope.scopeModel.headers.push(header);
            };

            $scope.scopeModel.addParameter = function () {
                var parameter = {};
                $scope.scopeModel.parameters.push(parameter);
            };

            $scope.scopeModel.addURLParameter = function () {
                var urlParameter = {};
                $scope.scopeModel.urlParameters.push(urlParameter);
            };

            $scope.scopeModel.validateHeaders = function () {
                var names = [];
                for (var i = 0; i < $scope.scopeModel.headers.length; i++) {
                    var name = $scope.scopeModel.headers[i].name;
                    if (name != undefined) {
                        names.push(name);
                    }
                }
                while (names.length > 0) {
                    var nameToValidate = names[0];
                    names.splice(0, 1);
                    if (!validateName(nameToValidate, names)) {
                        return 'Two or more headers have the same name.';
                    }
                }
                return null;
            };
            $scope.scopeModel.validateURLParameters = function () {
                var names = [];
                for (var i = 0; i < $scope.scopeModel.urlParameters.length; i++) {
                    var name = $scope.scopeModel.urlParameters[i].name;
                    if (name != undefined) {
                        names.push(name);
                    }
                }
                while (names.length > 0) {
                    var nameToValidate = names[0];
                    names.splice(0, 1);
                    if (!validateName(nameToValidate, names)) {
                        return 'Two or more URL parameters have the same name.';
                    }
                }
                return null;
            };
            $scope.scopeModel.validateParameters = function () {
                var names = [];
                var includedInBodyParams = [];
                var includeInBody = false;
                for (var i = 0; i < $scope.scopeModel.parameters.length; i++) {
                    var parameter = $scope.scopeModel.parameters[i];
                    var name = parameter.name;
                    if (parameter.includeInBody) {
                        includeInBody = true;
                        includedInBodyParams.push(parameter.includeInBody);
                    }
                    if (name != undefined) {
                        names.push(name);
                    }
                }
                if (includeInBody) {
                    $scope.scopeModel.isBodyDisabled = true;
                }
                else {
                    $scope.scopeModel.isBodyDisabled = false;
                }
                while (names.length > 0) {
                    var nameToValidate = names[0];
                    names.splice(0, 1);
                    if (!validateName(nameToValidate, names)) {
                        return 'Two or more parameters have the same name.';
                    }
                }
                while (includedInBodyParams.length > 0) {
                    var included = includedInBodyParams[0];
                    includedInBodyParams.splice(0, 1);
                    if (!validateParam(included, includedInBodyParams)) {
                        return 'Two or more parameters are taken as the single body parameter.';
                    }
                }
                return null;
            };

            function validateName(name, array) {
                for (var j = 0; j < array.length; j++) {
                    if (array[j] == name) {
                        return false;
                    }
                }
                return true;
            }

            function validateParam(included, array) {
                for (var j = 0; j < array.length; j++) {
                    if (array[j] == included && included) {
                        return false;
                    }
                }
                return true;
            }

            $scope.scopeModel.onDeleteHeader = function (header) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.headers, header.name, 'name');
                if (index > -1) {
                    $scope.scopeModel.headers.splice(index, 1);
                }
            };

            $scope.scopeModel.onDeleteURLParameter = function (urlParameter) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.urlParameters, urlParameter.name, 'name');
                if (index > -1) {
                    $scope.scopeModel.urlParameters.splice(index, 1);
                }
            };

            $scope.scopeModel.onDeleteParameter = function (parameter) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.parameters, parameter.name, 'name');
                if (index > -1) {
                    $scope.scopeModel.parameters.splice(index, 1);
                }
            };
            $scope.scopeModel.save = function () {
                if (!isEditMode)
                    insertHttpProxyMethod();
                else
                    updateHttpProxyMethod();
            };

        }

        $scope.scopeModel.close = function () {
            $scope.modalContext.closeModal();
        };


        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {

                function setTitle() {
                    if (isEditMode)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(httpProxyMethodEntity.MethodName, "Http Proxy Method");
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor("Http Proxy Method");
                }
                function loadStaticData() {
                    if (httpProxyMethodEntity == undefined)
                        return;
                    $scope.scopeModel.selectedHttpMethodType = UtilsService.getItemByVal($scope.scopeModel.httpMethodTypes, httpProxyMethodEntity.MethodType, 'value');
                    $scope.scopeModel.selectedHttpMethodFormat = UtilsService.getItemByVal($scope.scopeModel.httpMessageFormats, httpProxyMethodEntity.MessageFormat, 'value');
                    $scope.scopeModel.methodName = httpProxyMethodEntity.MethodName;
                    $scope.scopeModel.actionPath = httpProxyMethodEntity.ActionPath;
                    $scope.scopeModel.returnType = httpProxyMethodEntity.ReturnType;
                    $scope.scopeModel.body = httpProxyMethodEntity.Body;
                    $scope.scopeModel.responseLogic = httpProxyMethodEntity.ResponseLogic;
                    $scope.scopeModel.classMembers = httpProxyMethodEntity.ClassMembers;
                    $scope.scopeModel.namespaceMembers = httpProxyMethodEntity.NamespaceMembers;

                    if (httpProxyMethodEntity.Headers != undefined) {
                        for (var headerName in httpProxyMethodEntity.Headers) {
                            if (headerName != "$type") {
                                $scope.scopeModel.headers.push({
                                    name: headerName,
                                    value: httpProxyMethodEntity.Headers[headerName]
                                });
                            }
                        }
                    }
                    if (httpProxyMethodEntity.Parameters != undefined && httpProxyMethodEntity.Parameters.length > 0) {
                        for (var i = 0; i < httpProxyMethodEntity.Parameters.length; i++) {
                            var parameter = httpProxyMethodEntity.Parameters[i];
                            $scope.scopeModel.parameters.push({
                                name: parameter.ParameterName,
                                type: parameter.ParameterType,
                                includeInHeader: parameter.IncludeInHeader,
                                includeInBody: parameter.IncludeInBody,
                                includeInURL: parameter.IncludeInURL
                            });
                        }
                    }
                    if (httpProxyMethodEntity.URLParameters != undefined) {
                        for (var urlParameterName in httpProxyMethodEntity.URLParameters) {
                            if (urlParameterName != "$type") {
                                $scope.scopeModel.urlParameters.push({
                                    name: urlParameterName,
                                    value: httpProxyMethodEntity.URLParameters[urlParameterName]
                                });
                            }
                        }
                    }
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

        }
        function buildObjectFromScope() {
            function getHeaders() {
                var headers = {};
                if ($scope.scopeModel.headers.length > 0) {
                    for (var i = 0; i < $scope.scopeModel.headers.length; i++) {
                        var header = $scope.scopeModel.headers[i];
                        headers[header.name] = header.value;
                    }
                }
                return headers;
            }
            function getParameters() {
                var parameters = [];
                if ($scope.scopeModel.parameters.length > 0) {
                    for (var i = 0; i < $scope.scopeModel.parameters.length; i++) {
                        var parameter = $scope.scopeModel.parameters[i];
                        parameters.push({
                            ParameterName: parameter.name,
                            ParameterType: parameter.type,
                            IncludeInHeader: parameter.includeInHeader,
                            IncludeInBody: parameter.includeInBody,
                            IncludeInURL: parameter.includeInURL
                        });
                    }
                }
                return parameters;
            }
            function getURLParameters() {
                var urlParameters = {};
                if ($scope.scopeModel.urlParameters.length > 0) {
                    for (var i = 0; i < $scope.scopeModel.urlParameters.length; i++) {
                        var urlParameter = $scope.scopeModel.urlParameters[i];
                        urlParameters[urlParameter.name] = urlParameter.value;
                    }
                }
                return urlParameters;
            }
            return {
                MethodName: $scope.scopeModel.methodName,
                MethodType: $scope.scopeModel.selectedHttpMethodType.value,
                MessageFormat: $scope.scopeModel.selectedHttpMethodFormat.value,
                ActionPath: $scope.scopeModel.actionPath,
                ReturnType: $scope.scopeModel.returnType,
                Body: $scope.scopeModel.body,
                Headers: getHeaders(),
                Parameters: getParameters(),
                URLParameters: getURLParameters(),
                ResponseLogic: $scope.scopeModel.responseLogic,
                ClassMembers: $scope.scopeModel.classMembers,
                NamespaceMembers: $scope.scopeModel.namespaceMembers
            };
        }
        function insertHttpProxyMethod() {
            var httpProxyMethod = buildObjectFromScope();
            if ($scope.onHttpProxyMethodAdded != undefined && typeof ($scope.onHttpProxyMethodAdded)=="function")
                $scope.onHttpProxyMethodAdded(httpProxyMethod);
            $scope.modalContext.closeModal();
        }

        function updateHttpProxyMethod() {
            var httpProxyMethod = buildObjectFromScope();
            if ($scope.onHttpProxyMethodUpdated != undefined && typeof ($scope.onHttpProxyMethodUpdated)=="function")
                $scope.onHttpProxyMethodUpdated(httpProxyMethod);
            $scope.modalContext.closeModal();

        }
    }
    appControllers.controller('VRCommon_HttpProxyMethodEditorController', httpProxyMethodEditorController);
})(appControllers);