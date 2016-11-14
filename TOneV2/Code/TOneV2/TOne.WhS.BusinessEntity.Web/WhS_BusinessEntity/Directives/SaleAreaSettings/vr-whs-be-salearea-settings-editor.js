'use strict';

app.directive('vrWhsBeSaleareaSettingsEditor', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_PrimarySaleEntityEnum',
    function (UtilsService, VRUIUtilsService, WhS_BE_PrimarySaleEntityEnum) {

    	var directiveDefinitionObject = {
    		restrict: 'E',
    		scope: {
    			onReady: '='
    		},
    		controller: function ($scope, $element, $attrs) {
    			var ctrl = this;
    			var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
    			ctor.initializeController();
    		},
    		controllerAs: 'ctrl',
    		bindToController: true,
    		compile: function (element, attrs) {

    		},
    		templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SaleAreaSettings/Templates/SaleAreaSettingsTemplate.html"
    	};

    	function settingEditorCtor(ctrl, $scope, $attrs) {

    		this.initializeController = initializeController;

    		ctrl.fixedKeywords = [];
    		ctrl.mobileKeywords = [];
    		ctrl.primarySaleEntities = UtilsService.getArrayEnum(WhS_BE_PrimarySaleEntityEnum);

    		var mailMsgTemplateSelectorAPI;
    		var mailMsgTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

    		var salePLTemplateSelectorAPI;
    		var salePLTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

    		function initializeController() {
    			ctrl.disabledAddFixedKeyword = true;
    			ctrl.disabledAddMobileKeyword = true;

    			ctrl.addFixedKeyword = function () {
    				ctrl.fixedKeywords.push({ fixedKeyword: ctrl.fixedKeywordvalue });
    				ctrl.fixedKeywordvalue = undefined;
    				ctrl.disabledAddFixedKeyword = true;
    			};

    			ctrl.onFixedKeywordValueChange = function (value) {
    				ctrl.disabledAddFixedKeyword = (value == undefined && ctrl.fixedKeywordvalue.length - 1 < 1) || UtilsService.getItemIndexByVal(ctrl.fixedKeywords, value, "fixedKeyword") != -1;
    			};

    			$scope.onPriceListEmailTemplateSelectorReady = function (api) {
    				mailMsgTemplateSelectorAPI = api;
    				mailMsgTemplateSelectorReadyDeferred.resolve();
    			}
    			ctrl.validateAddFixedKeyWords = function () {
    				if (ctrl.fixedKeywords != undefined && ctrl.fixedKeywords.length == 0)
    					return "Enter at least one keyword.";
    				return null;
    			};


    			ctrl.addMobileKeyword = function () {
    				ctrl.mobileKeywords.push({ mobileKeyword: ctrl.mobileKeywordvalue });
    				ctrl.mobileKeywordvalue = undefined;
    				ctrl.disabledAddMobileKeyword = true;
    			};

    			ctrl.onMobileKeywordValueChange = function (value) {
    				ctrl.disabledAddMobileKeyword = (value == undefined && ctrl.mobileKeywordvalue.length - 1 < 1) || UtilsService.getItemIndexByVal(ctrl.mobileKeywords, value, "mobileKeyword") != -1;
    			};

    			ctrl.validateAddMobileKeyWords = function () {
    				if (ctrl.mobileKeywords != undefined && ctrl.mobileKeywords.length == 0)
    					return "Enter at least one keyword.";
    				return null;
    			};

    			ctrl.onPrimarySaleEntitySelectorReady = function (api) {

    				return mailMsgTemplateSelectorReadyDeferred.promise.then(function () {
    					defineAPI();
    				});
    			};

    			ctrl.onSalePLTemplateSelectorReady = function (api) {
    				salePLTemplateSelectorAPI = api;
    				salePLTemplateSelectorReadyDeferred.resolve();
    			};
    		}

    		function defineAPI() {

    			var api = {};

    			api.load = function (payload) {

    				var promises = [];

    				var data;
    				var selectedMailMsgTemplateId;
    				var selectedSalePLTemplateId;

    				if (payload != undefined) {
    					data = payload.data;
    				}

    				if (data != undefined) {
    					selectedMailMsgTemplateId = data.DefaultSalePLMailTemplateId;
    					selectedSalePLTemplateId = data.DefaultSalePLTemplateId;
    				}

    				loadStaticData(data);

    				var loadMailMsgTemplateSelectorPromise = loadMailMsgTemplateSelector(selectedMailMsgTemplateId);
    				promises.push(loadMailMsgTemplateSelectorPromise);

    				var loadSalePLTemplateSelectorPromise = loadSalePLTemplateSelector(selectedSalePLTemplateId);
    				promises.push(loadSalePLTemplateSelectorPromise);

    				return UtilsService.waitMultiplePromises(promises);
    			};

    			api.getData = function () {
    				return {
    					$type: "TOne.WhS.BusinessEntity.Entities.SaleAreaSettingsData, TOne.WhS.BusinessEntity.Entities",
    					FixedKeywords: UtilsService.getPropValuesFromArray(ctrl.fixedKeywords, "fixedKeyword"),
    					MobileKeywords: UtilsService.getPropValuesFromArray(ctrl.mobileKeywords, "mobileKeyword"),
    					DefaultRate: ctrl.defaultRate,
    					PrimarySaleEntity: ctrl.primarySaleEntity.value,
    					DefaultSalePLMailTemplateId: mailMsgTemplateSelectorAPI.getSelectedIds(),
    					DefaultSalePLTemplateId: salePLTemplateSelectorAPI.getSelectedIds()
    				};
    			};

    			if (ctrl.onReady != null)
    				ctrl.onReady(api);
    		}

    		function loadStaticData(data) {

    			if (data == undefined)
    				return;

    			ctrl.defaultRate = data.DefaultRate;
    			ctrl.primarySaleEntity = UtilsService.getItemByVal(ctrl.primarySaleEntities, data.PrimarySaleEntity, 'value');

    			if (data.FixedKeywords != null) {
    				for (var i = 0; i < data.FixedKeywords.length; i++)
    					ctrl.fixedKeywords.push({ fixedKeyword: data.FixedKeywords[i] });
    			}
    			if (data.MobileKeywords != null) {
    				for (var i = 0; i < data.MobileKeywords.length; i++)
    					ctrl.mobileKeywords.push({ mobileKeyword: data.MobileKeywords[i] });
    			}
    		}
    		function loadMailMsgTemplateSelector(selectedId) {
    			var mailMsgTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
    			mailMsgTemplateSelectorReadyDeferred.promise.then(function () {
    				var mailMsgTemplateSelectorPayload = {
    					selectedIds: selectedId,
    					filter: {
    						VRMailMessageTypeId: "f61f0b87-ee5b-4794-8b0f-6c0777006441",
    					}
    				}
    				VRUIUtilsService.callDirectiveLoad(mailMsgTemplateSelectorAPI, mailMsgTemplateSelectorPayload, mailMsgTemplateSelectorLoadDeferred);
    			});
    			return mailMsgTemplateSelectorLoadDeferred.promise;
    		}
    		function loadSalePLTemplateSelector(selectedId) {
    			var salePLTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
    			salePLTemplateSelectorReadyDeferred.promise.then(function () {
    				var salePLTemplateSelectorPayload = {
    					selectedIds: selectedId
    				};
    				VRUIUtilsService.callDirectiveLoad(salePLTemplateSelectorAPI, salePLTemplateSelectorPayload, salePLTemplateSelectorLoadDeferred);
    			});
    			return salePLTemplateSelectorLoadDeferred.promise;
    		}
    	}

    	return directiveDefinitionObject;
    }]);