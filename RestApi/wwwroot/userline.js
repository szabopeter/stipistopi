function UserLineViewModel() {
    this.userName = ko.observable("TODO: userName");
    this.role = ko.observable("TODO: role");
    this.actions = ko.observableArray([]);

    this.updateActions = function (backend, refresh) {
        // TODO:
    };
}

UserLineViewModel.create = function (source) {
    let vm = new UserLineViewModel();
    vm.userName(source.userName);
    vm.role(source.role);
    return vm;
}

function userLineRegisterWidget(template) {
    let widgetName = "userline-widget";
    ko.components.register(widgetName, {
        viewModel: function(params) {return params;},
        template: template,
    });
    return widgetName;
}

export { UserLineViewModel, userLineRegisterWidget }