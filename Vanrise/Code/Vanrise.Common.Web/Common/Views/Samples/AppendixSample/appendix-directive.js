﻿'use strict';
app.directive('vrCommonAppendixsampleAppendix', ['UtilsService', 'Common_AppendixSample_Service',
function (UtilsService, Common_AppendixSample_Service) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.dataSource = [];
                ctrl.selectedValue = [];
                //ctrl.selectReady = function (api) {
                    var api = {};
                    api.load = function (payload) {
                        return Common_AppendixSample_Service.getRemoteData(2000)
                            .then(function (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.dataSource.push(response[i]);
                            }
                            if (payload != undefined)
                                ctrl.selectedValue = ctrl.dataSource[0];
                        });
                    };
                    setTimeout(function () {
                        if (ctrl.onReady != undefined)
                            ctrl.onReady(api);
                    }, 100);
              //  };
              
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return '<vr-select datatextfield="description" datavaluefield="value" label="Select Appendix 1" datasource="ctrl.dataSource"'
                +'selectedvalues="ctrl.selectedValue" on-ready="ctrl.selectReady" isrequired="true"></vr-columns>';
            }

        };

        
        return directiveDefinitionObject;
    }]);