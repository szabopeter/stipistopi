function ResourceLineViewModel() {
    let self = this;
    this.shortName = ko.observable("TODO: shortName");
    this.address = ko.observable("TODO: address");
    this.isAvailable = ko.observable("TODO: isAvailable");
    this.lockedBy = ko.observable("TODO: lockedBy");
    this.actions = ko.observableArray([]);
    this.messageManagerVm = null;

    this.updateActions = function (backend, refresh) {
        let resource = this;
        let label = resource.isAvailable() ? "Lock" : "Release";
        let backendAction = resource.isAvailable() ? "lock" : "release";
        let execute = function () {
            backend.sendResourceAction(backendAction, resource.shortName(), function (data) {
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

ResourceLineViewModel.create = function (source) {
    let vm = new ResourceLineViewModel();
    vm.shortName(source.shortName);
    vm.address(source.address);
    vm.isAvailable(source.isAvailable);
    vm.lockedBy(source.lockedBy);
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