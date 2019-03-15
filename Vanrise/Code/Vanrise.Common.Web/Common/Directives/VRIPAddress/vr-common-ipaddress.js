'use strict';

app.directive('vrCommonIpaddress', ['UtilsService', 'VRUIUtilsService', 'VRCommon_IPAddressTypeEnum',
    function (UtilsService, VRUIUtilsService, VRCommon_IPAddressTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                //normalColNum: '@',
                ondeselectitem: '=',
                isrequired: '=',
                hidelabel: '@',
                hidesubnetprefix: '@',
                gridlayout: '@',
                onipaddressblur: '=',
                onprefixblur: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                var ctor = new IPAddressCtor(ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, $attrs) {
                return getTemplate($attrs);
            }
        };

        function IPAddressCtor(ctrl, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                if (attrs.gridlayout != undefined) {
                    ctrl.ipTypeColNum = 3;
                    ctrl.ipv4AddressColNum = (attrs.hidesubnetprefix == undefined) ? 6 : 9;
                    ctrl.ipv6AddressColNum = 9;
                    ctrl.subnetPrefixColNum = 3;
                } else {
                    ctrl.ipTypeColNum = 2;
                    ctrl.ipv4AddressColNum = 4;
                    ctrl.ipv6AddressColNum = 6;
                    ctrl.subnetPrefixColNum = 2;
                }

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                ctrl.onDeSelectType = function () {

                    if (ctrl.ondeselectitem != undefined && typeof (ctrl.ondeselectitem) == "function") {
                        ctrl.ondeselectitem();
                        return;
                    }

                    ctrl.ipAddress = undefined;
                    ctrl.subnetPrefixLength = undefined;
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var ipAddresstype;

                    if (payload != undefined) {
                        ipAddresstype = payload.Type;
                        ctrl.ipAddress = payload.IPAddress;
                        ctrl.subnetPrefixLength = payload.SubnetPrefixLength;
                    }

                    var ipAddressTypes = UtilsService.getArrayEnum(VRCommon_IPAddressTypeEnum);
                    for (var i = 0; i < ipAddressTypes.length; i++) {
                        ctrl.datasource.push(ipAddressTypes[i]);
                    }

                    if (ipAddresstype != undefined) {
                        VRUIUtilsService.setSelectedValues(ipAddresstype, 'value', attrs, ctrl);
                    } else if (attrs.hidelabel != undefined) {
                        VRUIUtilsService.setSelectedValues(ipAddressTypes[0].value, 'value', attrs, ctrl);
                    }
                };

                api.getData = function () {

                    var ipAddressType = ctrl.selectedvalues != undefined ? ctrl.selectedvalues.value : undefined;
                    if (ipAddressType == undefined)
                        return;

                    return {
                        Type: ipAddressType,
                        IPAddress: ctrl.ipAddress,
                        SubnetPrefixLength: (ipAddressType != VRCommon_IPAddressTypeEnum.IPv6.value) ? ctrl.subnetPrefixLength : undefined
                    };
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {
            var rightMediumSpacing = "";
            var mediumSpacing = "";
            var hideRemoveIcon = "";
            var onIPAddressBlur = "";
            var onPrefixBlur = "";

            var typeLabel = "";
            var ipLabel = "";
            var subnetLabel = "";

            var ipPlaceHolder = "";
            var subnetPlaceHolder = "";

            var hidelabel = "";
            var showSubnetPrefix = true;

            if (attrs.hidelabel == undefined) {
                typeLabel = "IP Type";
                ipLabel = "IP Address";
                subnetLabel = "Subnet Prefix Length";
            } else {
                hidelabel = "hidelabel";
                ipPlaceHolder = "IP Address";
                subnetPlaceHolder = "Prefix Size";
            }

            if (attrs.gridlayout != undefined) {
                rightMediumSpacing = "haschildcolumns right-medium-spacing";
                mediumSpacing = "medium-spacing";
            }

            if (attrs.isrequired != undefined)
                hideRemoveIcon = "hideremoveicon";

            if (attrs.hidesubnetprefix != undefined)
                showSubnetPrefix = false;

            if (attrs.onipaddressblur != undefined)
                onIPAddressBlur = 'onblurtextbox = "ctrl.onipaddressblur(ctrl.ipAddress)"';

            if (attrs.onprefixblur != undefined)
                onPrefixBlur = 'onblurtextbox = "ctrl.onprefixblur(ctrl.subnetPrefixLength)"';

            return '<vr-columns colnum="{{ctrl.ipTypeColNum}}" ' + rightMediumSpacing + '> ' +
                ' <vr-select on-ready="ctrl.onSelectorReady"' +
                ' datasource="ctrl.datasource"' +
                ' datatextfield="description"' +
                ' datavaluefield="value"' +
                ' label="' + typeLabel + '"' +
                ' ' + hidelabel +
                ' isrequired="ctrl.isrequired"' +
                ' selectedvalues="ctrl.selectedvalues"' +
                ' onselectitem="ctrl.onselectitem"' +
                ' ondeselectitem="ctrl.onDeSelectType"' +
                ' entityName="IP Type" ' + hideRemoveIcon + '> ' +
                ' </vr-select> ' +
                '  </vr-columns> ' +

                ' <vr-columns colnum="{{ctrl.ipv4AddressColNum}}" ng-if="ctrl.selectedvalues == undefined || ctrl.selectedvalues.value == 0" ' + mediumSpacing + '> ' +
                ' <span ng-if="ctrl.hidelabel == undefined">' +
                ' <vr-label>' + ipLabel + '</vr-label>' +
                ' </span> ' +
                ' <vr-textbox type="ip" value="ctrl.ipAddress" ' + onIPAddressBlur + ' placeholder=" ' + ipPlaceHolder + ' " isrequired="ctrl.isrequired"></vr-textbox> ' +
                ' </vr-columns> ' +

                ' <vr-columns colnum="{{ctrl.ipv6AddressColNum}}" ng-if="ctrl.selectedvalues != undefined && ctrl.selectedvalues.value == 1" ' + mediumSpacing + '> ' +
                ' <span ng-if="ctrl.hidelabel == undefined">' +
                ' <vr-label>' + ipLabel + '</vr-label>' +
                ' </span > ' +
                ' <vr-textbox type="ipv6" value="ctrl.ipAddress" ' + onIPAddressBlur + ' placeholder=" ' + ipPlaceHolder + ' " isrequired="ctrl.isrequired"></vr-textbox> ' +
                ' </vr-columns> ' +

                ' <vr-columns colnum="{{ctrl.subnetPrefixColNum}}" ng-if="' + showSubnetPrefix + ' && ctrl.selectedvalues != undefined && ctrl.selectedvalues.value == 0" ' + mediumSpacing + '> ' +
                ' <span ng-if="ctrl.hidelabel == undefined">' +
                '   <vr-label>' + subnetLabel + '</vr-label>' +
                ' </span > ' +
                ' <vr-textbox type="number" minvalue="0" maxvalue="32" value="ctrl.subnetPrefixLength" ' + onPrefixBlur +
                '  placeholder="' + subnetPlaceHolder + '" isrequired="ctrl.isrequired" ></vr-textbox> ' +
                ' </vr-columns> ';
        }
    }]);