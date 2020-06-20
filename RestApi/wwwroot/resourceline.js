import { DescriptionEditorViewModel } from "./descriptioneditor.js";

function ResourceLineViewModel(backend, messageManagerVm, updateButtonsState) {
    let self = this;
    this.backend = backend;
    this.messageManagerVm = messageManagerVm;
    this.updateButtonsState = updateButtonsState;
    this.shortName = ko.observable("TODO: shortName");
    this.address = ko.observable("TODO: address");
    this.descriptionEditorVm = new DescriptionEditorViewModel(backend, messageManagerVm, updateButtonsState);
    this.isAvailable = ko.observable(true);
    this.lockedBy = ko.observable("");
    this.lockedAt = ko.observable("");
    this.actions = ko.observableArray([]);
    this.messageManagerVm = null;
    this.actionsEnabled = ko.observable(true);

    this.updateActions = function (refresh) {
        let resource = this;
        let label = resource.isAvailable() ? "Lock" : "Release";
        let backendAction = resource.isAvailable() ? "lock" : "release";
        let execute = function () {
            self.backend.sendResourceAction(backendAction, resource.shortName(), function (data) {
                refresh();
                if (data == true) {
                    self.messageManagerVm.showDisappearingMessage("OK");
                } else {
                    self.messageManagerVm.showMessageWithOk("Failed!");
                };
            }, refresh);
        };
        resource.actions([{
            label: label,
            backendAction: backendAction,
            execute: execute,
        }]);
    };
}

ResourceLineViewModel.create = function (ssResource, backend, messageManagerVm, updateButtonsState) {
    let vm = new ResourceLineViewModel(backend, messageManagerVm, updateButtonsState);
    vm.shortName(ssResource.shortName);
    vm.address(ssResource.address);
    vm.descriptionEditorVm.resourceName = ssResource.shortName;
    vm.descriptionEditorVm.oldDescription(ssResource.description);
    vm.descriptionEditorVm.newDescription(ssResource.description);
    if (ssResource.locking != null) {
        vm.isAvailable(false);
        vm.lockedBy(ssResource.locking.lockedBy.userName);
        vm.lockedAt(ssResource.ui.lockedAt);
    }
    // vm.actions(source.actions);
    // vm.actions().forEach(function (action) { action.execute = action.execute.bind(vm); })
    return vm;
}

function resourceLineRegisterWidget(template) {
    let widgetName = "resourceline-widget";
    ko.components.register(widgetName, {
        viewModel: function (params) { return params; },
        template: template,
    });
    return widgetName;
}

export { ResourceLineViewModel, resourceLineRegisterWidget }